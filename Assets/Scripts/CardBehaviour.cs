using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class CardBehaviour : Draggable
{
    private static Sprite[] avatars;
    private static Sprite[] backgrounds;
    static private bool isDetailView = false;
    private bool isBackView = false;
    private string initialSortingLayer;
    private SortingGroup sortingGroup;
    private SpriteRenderer spriteRenderer;
    private bool wasLocked;
    private Vector3 positionBeforeDetailedView;
    private bool isMoving = false;
    private bool isSpawned = false;
    private int turnsFromSpawn = 0;
    private Vector3 startLocalScale;
    private int startHealth;
    public bool isUFOed = false;

    public int TurnsFromSpawn { get { return turnsFromSpawn; } }

    public CardData Data { get; set; }

    public void LoadData(CardData data)
    {
        Data = data;
        SetCardText("Cost", (data.cost).ToString());
        SetCardText("Title", data.title);
        SetCardText("Description", data.description);
        SetCardText("Attack", (data.attack).ToString());
        SetCardText("Health", (data.health).ToString());
        SetCardAvatar(data.avatarNumber);
        BackView(true);
        startHealth = data.health;
        isLocked = data.isReptilian;
        wasLocked = isLocked;
    }

    public void BackView(bool isActive)
    {
        isBackView = isActive;
        if (isActive)
        {
            isLocked = true;
            spriteRenderer.sortingOrder = 2;
            spriteRenderer.sprite = Data.isReptilian ? backgrounds[2] : backgrounds[1];
        }
        else
        {
            isLocked = wasLocked;
            spriteRenderer.sortingOrder = 0;
            spriteRenderer.sprite = backgrounds[0];
        }
    }

    private void SetCardAvatar(int avatarNumber)
    {
        Transform child = transform.Find("Avatar");
        child.gameObject.GetComponent<SpriteRenderer>().sprite = avatars[avatarNumber];
    }

    private void SetCardText(string name, string value)
    {
        Transform child = transform.Find(name);
        child.gameObject.GetComponent<TextMeshPro>().text = value;
    }

    protected override void OnAwake()
    {
        backgrounds ??= Resources.LoadAll<Sprite>("backgrounds");
        avatars ??= Resources.LoadAll<Sprite>("avatars");
        sortingGroup = GetComponent<SortingGroup>();
        initialSortingLayer = sortingGroup.sortingLayerName;
        spriteRenderer = GetComponent<SpriteRenderer>();
        startLocalScale = transform.localScale;
    }

    private void OnEnable()
    {
        EventBus.Instance.OnTurnEnded += IncreaseTurnsFromSpawn;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnTurnEnded -= IncreaseTurnsFromSpawn;
    }

    private void LayerOnTop(bool isOnTop)
    {
        if (isOnTop)
        {
            sortingGroup.sortingLayerName = Constants.LAYER_ON_TOP;
        }
        else
        {
            sortingGroup.sortingLayerName = initialSortingLayer;
        }
    }

    private void DetailView(bool isActive)
    {
        if (isBackView || isMoving) return;

        if (isActive)
        {
            if (isDetailView) return;
            LayerOnTop(true);
            transform.localScale = new Vector3(
                transform.localScale.x * 1.5f,
                transform.localScale.y * 1.5f,
                transform.localScale.z);
            positionBeforeDetailedView = transform.position;
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y + 0.5f,
                transform.position.z);
            isDetailView = true;
        }
        else
        {
            if (!isDetailView) return;
            LayerOnTop(false);
            transform.localScale = startLocalScale;
            transform.position = positionBeforeDetailedView;
            isDetailView = false;
        }
    }

    private void OnMouseEnter()
    {
        if (draggedNow != null || isBackView) return;
        DetailView(true);
    }

    private void OnMouseExit()
    {
        DetailView(false);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (isDetailView) DetailView(false);
        LayerOnTop(true);
        base.OnBeginDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        LayerOnTop(false);
        base.OnEndDrag(eventData);
        if (isPlaced)
        {
            isSpawned = true;
            CardBehaviour cardBehaviour = gameObject.GetComponent<CardBehaviour>();
            target.GetComponent<SlotBehaviour>().Card = cardBehaviour;
            EventBus.Instance.RaiseOnCardPlaced(cardBehaviour, Data.isReptilian);
        }
    }

    protected override bool CanBePlaced()
    {
        return target.GetComponent<SlotBehaviour>().CanAcceptCard(Data);
    }

    public IEnumerator MoveAndRotateAnimation(Vector3 endPosition, Quaternion endRotation, float duration)
    {
        isMoving = true;
        float time = 0;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        while (time < duration)
        {
            time += Time.deltaTime;
            float normalizedTime = time / duration;
            transform.SetPositionAndRotation(Vector3.Lerp(startPosition, endPosition, normalizedTime),
                Quaternion.Lerp(startRotation, endRotation, normalizedTime));
            yield return null;
        }

        transform.SetPositionAndRotation(endPosition, endRotation);
        isMoving = false;
    }

    public void StartMoveAndRotateAnimation(Vector3 endPosition, Quaternion endRotation)
    {
        StartCoroutine(MoveAndRotateAnimation(endPosition, endRotation, Constants.TIME_GET_CARD_ANIMATION));
    }

    private IEnumerator HandOverflowAnimation(float duration)
    {
        isMoving = true;
        float time = 0;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new(0, 0, 0);
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.identity;

        // display in the center of the screen
        while (time < duration)
        {
            time += Time.deltaTime;
            float normalizedTime = time / duration;
            transform.SetPositionAndRotation(Vector3.Lerp(startPosition, endPosition, normalizedTime),
                Quaternion.Lerp(startRotation, endRotation, normalizedTime));
            yield return null;
        }
        transform.SetPositionAndRotation(endPosition, endRotation);

        // destroy
        yield return DestroyCardWithAnimation(duration);
        EventBus.Instance.RaiseOnHandOverflow(Data.isReptilian);
    }

    public void StartHandOverflowAnimation()
    {
        StartCoroutine(HandOverflowAnimation(Constants.TIME_GET_CARD_ANIMATION));
    }

    private IEnumerator DestroyCardWithAnimation(float duration)
    {
        isMoving = true;
        float time = 0;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, 180);
        Vector3 startScale = transform.localScale;
        Vector3 endScale = new(0, 0, 0);
        yield return new WaitForSeconds(duration);
        while (time < duration)
        {
            time += Time.deltaTime;
            float normalizedTime = time / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, normalizedTime);
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, normalizedTime);
            yield return null;
        }
        Destroy(gameObject);
    }

    public void StartDestroyCardWithAnimation()
    {
        StartCoroutine(DestroyCardWithAnimation(Constants.TIME_DESTROY_CARD_ANIMATION));
    }

    private IEnumerator AttackAnimation(float duration)
    {
        isMoving = true;
        float time = 0;
        float deltaAttack = Data.isReptilian ? -1f : 1f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + new Vector3(0f, deltaAttack, 0f);

        // attack
        while (time < duration / 3)
        {
            time += Time.deltaTime;
            float normalizedTime = time / duration;
            transform.position = Vector3.Lerp(startPosition, endPosition, normalizedTime);
            yield return null;
        }
        transform.position = endPosition;

        // go back
        while (time < duration * 2 / 3)
        {
            time += Time.deltaTime;
            float normalizedTime = time / duration;
            transform.position = Vector3.Lerp(endPosition, startPosition, normalizedTime);
            yield return null;
        }
        transform.position = startPosition;
        isMoving = false;
    }

    public void StartAttackAnimation()
    {
        StartCoroutine(AttackAnimation(Constants.TIME_CARD_ATTACK_ANIMATION));
    }

    private IEnumerator Rotate3DAnimation(float duration)
    {
        isMoving = true;
        float time = 0;
        Quaternion startRotation = Quaternion.identity;
        Quaternion Rotation = Quaternion.Euler(0, 90f, 0);
        Quaternion halfRotation = Quaternion.Euler(0, 90f, 0);

        time = 0;
        while (time < duration / 4)
        {
            time += Time.deltaTime;
            float normalizedTime = time / duration;
            transform.rotation = Quaternion.Lerp(startRotation, Quaternion.Euler(0, 90f, 0), normalizedTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 90f, 0);

        BackView(true);
        time = 0;
        while (time < duration / 4)
        {
            time += Time.deltaTime;
            float normalizedTime = time / duration;
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 90f, 0), Quaternion.Euler(0, 180f, 0), normalizedTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 180f, 0);

        time = 0;
        while (time < duration / 4)
        {
            time += Time.deltaTime;
            float normalizedTime = time / duration;
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 180f, 0), Quaternion.Euler(0, 270, 0), normalizedTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 270f, 0);

        BackView(false);
        time = 0;
        while (time < duration / 4)
        {
            time += Time.deltaTime;
            float normalizedTime = time / duration;
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 270f, 0), Quaternion.identity, normalizedTime);
            yield return null;
        }
        transform.rotation = Quaternion.identity;
        isMoving = false;
    }

    public void StartRotate3DAnimation()
    {
        StartCoroutine(Rotate3DAnimation(Constants.TIME_ROTATE3D_ANIMATION));
    }

    public int TakeDamage(int damage)
    {
        Data.health -= damage;
        SetCardText("Health", (Data.health).ToString());
        if (Data.health <= 0) StartDestroyCardWithAnimation();
        return Data.health;
    }

    public IEnumerator Aberrate(bool aberrateCost = false)
    {
        if (Data.ability != CardData.Ability.FoilHat)
        {
            StartRotate3DAnimation();
            yield return new WaitForSeconds(Constants.TIME_ROTATE3D_ANIMATION / 2);
            int fate = Random.Range(0, 3); // generates 0, 1, or 2 to determine the outcome

            // success scenarios
            if (fate == 0 || fate == 1)
            {
                int successType = Random.Range(0, 5); // more granularity in success outcome
                if (successType == 0)
                {
                    // rarer success case, +1 to both attack and health
                    Data.attack += 1;
                    Data.health += 1;
                    if (aberrateCost) Data.cost -= 1;
                }
                else
                {
                    // more common, +1 to either attack or health
                    if (successType == 1 || successType == 2) // 50% chance
                    {
                        Data.attack += 1;
                    }
                    else
                    {
                        Data.health += 1;
                    }
                }
            }
            // failure scenarios
            else
            {
                int failureType = Random.Range(0, 5); // more granularity in failure outcome
                if (failureType == 0)
                {
                    // rarer failure case, -1 to both attack and health
                    Data.attack -= 1;
                    Data.health -= 1;
                    if (aberrateCost) Data.cost += 1;
                }
                else
                {
                    // more common, -1 to either attack or health
                    if (failureType == 1 || failureType == 2) // 50% chance
                    {
                        Data.attack -= 1;
                    }
                    else
                    {
                        Data.health -= 1;
                    }
                }
            }

            // modify one letter in the name
            int charPosition = Random.Range(0, Data.title.Length);
            char randomChar = (char)Random.Range(97, 123); // generates a random lowercase letter
            char[] titleArray = Data.title.ToCharArray();
            titleArray[charPosition] = randomChar;
            Data.title = new string(titleArray);

            // check boundaries
            Data.attack = System.Math.Max(1, Data.attack);
            Data.health = System.Math.Max(1, Data.health);
            Data.cost = System.Math.Max(1, Data.cost);

            // change view
            SetCardText("Title", Data.title);
            SetCardText("Attack", (Data.attack).ToString());
            SetCardText("Health", (Data.health).ToString());
            SetCardText("Cost", (Data.cost).ToString());
        }
    }

    public IEnumerator Mimic(int attack, int health)
    {
        StartRotate3DAnimation();
        yield return new WaitForSeconds(Constants.TIME_ROTATE3D_ANIMATION / 2);
        Data.attack = attack;
        Data.health = health;
        SetCardText("Attack", (Data.attack).ToString());
        SetCardText("Health", (Data.health).ToString());
    }

    public IEnumerator UFO()
    {
        EventBus.Instance.RaiseOnUFO(new Vector3(transform.position.x, transform.position.y + 1.1f, transform.position.z));
        yield return new WaitForSeconds(Constants.TIME_UFO_ANIMATION / 2);
        StartRotate3DAnimation();
        yield return new WaitForSeconds(Constants.TIME_ROTATE3D_ANIMATION / 2);
        Data.title = "<=>";
        Data.description = "A cow that has seen a lot\n-Immune to aberrations-";
        Data.attack = 2;
        Data.health = 2;
        Data.avatarNumber = 1;
        Data.ability = CardData.Ability.FoilHat;
        SetCardText("Title", Data.title);
        SetCardText("Description", Data.description);
        SetCardText("Attack", (Data.attack).ToString());
        SetCardText("Health", (Data.health).ToString());
        SetCardAvatar(Data.avatarNumber);
        isUFOed = true;
    }

    public IEnumerator Regenerate()
    {
        if (Data.health < startHealth) 
        {
            StartRotate3DAnimation();
            yield return new WaitForSeconds(Constants.TIME_ROTATE3D_ANIMATION / 2);
            Data.health += 1;
            SetCardText("Health", (Data.health).ToString());
        }
    }

    private void IncreaseTurnsFromSpawn()
    {
        if (isSpawned) turnsFromSpawn += 1;
    }
}
