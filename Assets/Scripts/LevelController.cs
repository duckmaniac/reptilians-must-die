using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelStates 
{
    PlayerTurn,
    ReptiliansTurn
};

public class LevelController : MonoBehaviour
{
    [SerializeField] private GameObject CardPrefab;
    [SerializeField] private GameObject PlayerSlots;
    [SerializeField] private GameObject ReptiliansSlots;
    public static LevelController Instance { get; private set; }
    private LevelData data;
    private Stack<CardBehaviour> playerDeck;
    private Stack<CardBehaviour> reptiliansDeck;
    private HandManager playerHand;
    private HandManager reptiliansHand;
    private List<SlotBehaviour> playerSlotBehaviours;
    private List<SlotBehaviour> reptiliansSlotBehaviours;
    private LevelStates state = LevelStates.PlayerTurn;
    private int playerSpawnPoints = 0;
    private int playerSpawnPointsUnused = 0;
    private int reptiliansSpawnPoints = 0;
    private int reptiliansSpawnPointsUnused = 0;
    private int playerHealth = 0;
    private int reptiliansHealth = 0;
    private int aiMode = 1;
    private int peaceCount = 0;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitLevel();
    }

    private void OnEnable()
    {
        EventBus.Instance.OnCardPlaced += RemoveCardFromHand;
        EventBus.Instance.OnCardPlaced += UseSpawnPoints;
        EventBus.Instance.OnTurnEnded += StartBattle;
        EventBus.Instance.OnTurnEnded += ForceEndDragging;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnCardPlaced -= RemoveCardFromHand;
        EventBus.Instance.OnCardPlaced -= UseSpawnPoints;
        EventBus.Instance.OnTurnEnded -= StartBattle;
        EventBus.Instance.OnTurnEnded -= ForceEndDragging;
    }

    private void InitLevel()
    {
        data = Utility.LoadJsonFromResources<LevelData>("LevelsData/" + GameManager.Instance.CurrentLevelNumber.ToString());
        List<CardData> reptiliansDeckData = LoadDeckData(data.reptiliansCardNumbers, GameManager.Instance.ReptiliansCardsData);
        List<CardData> playerDeckData = LoadDeckData(data.playerCardNumbers, GameManager.Instance.PlayerCardsData);
        ChangeHealth(data.playerHealth, false);
        ChangeHealth(data.reptilianBossHealth, true);
        playerDeck = LoadDeck(playerDeckData, Constants.POS_PLAYER_DECK);
        reptiliansDeck = LoadDeck(reptiliansDeckData, Constants.POS_REPTILIANS_DECK);
        playerHand = new HandManager(false);
        reptiliansHand = new HandManager(true);
        playerSlotBehaviours = LoadSlots(PlayerSlots);
        reptiliansSlotBehaviours = LoadSlots(ReptiliansSlots);
        aiMode = data.aiMode;
        EventBus.Instance.RaiseOnNameChanged(data.reptilianBossName, true);
        EventBus.Instance.RaiseOnBossAvatarChanged(data.bossAvatarNumber, true);
        if (data.level != 4)
        {
            StartCoroutine(PlayerTurn(4, true));
        } else
        {
            StartCoroutine(ReptiliansTurn(4,true));
        }
    }

    private void StartBattle()
    {
        EventBus.Instance.RaiseOnBattleStarted();
        StartCoroutine(Battle());
    }

    private IEnumerator Battle()
    {
        if (state == LevelStates.PlayerTurn)
        {
            yield return StartCoroutine(BattleProcess(false));
            StartCoroutine(ReptiliansTurn());
        }
        else
        {
            yield return StartCoroutine(BattleProcess(true));
            StartCoroutine(PlayerTurn());
        }
    }

    private IEnumerator BattleProcess(bool isReptiliansAttack)
    {
        List<SlotBehaviour> attackerSlots = isReptiliansAttack ? ref reptiliansSlotBehaviours : ref playerSlotBehaviours;
        List<SlotBehaviour> defenderSlots = isReptiliansAttack ? ref playerSlotBehaviours : ref reptiliansSlotBehaviours;
        for (int i = 0; i < attackerSlots.Count; i++)
        {
            if (attackerSlots[i].Card != null)
            {
                yield return ProcessAbilities(i, attackerSlots, defenderSlots);
            }
        }

        for (int i = 0; i < attackerSlots.Count; i++)
        {
            if (attackerSlots[i].Card != null)
            {
                CardBehaviour attackerCard = attackerSlots[i].Card;
                if (attackerCard.Data.attack > 0)
                {
                    attackerCard.StartAttackAnimation();
                    yield return new WaitForSeconds(2 * Constants.TIME_CARD_ATTACK_ANIMATION);
                }

                // if there is a card we attack it
                int damageToOpponent = 0;
                if (defenderSlots[i].Card != null)
                {
                    damageToOpponent = System.Math.Max(0, attackerCard.Data.attack - defenderSlots[i].Card.Data.health);
                    int cardHealth = defenderSlots[i].Card.TakeDamage(attackerCard.Data.attack);
                    if (cardHealth <= 0) defenderSlots[i].Card = null;
                }
                // if there is no card we attack the opponent
                else
                {
                    damageToOpponent = attackerCard.Data.attack;
                }
                if (damageToOpponent > 0)
                {
                    ChangeHealth(-damageToOpponent,!isReptiliansAttack);
                    EventBus.Instance.RaiseOnOpponentReceivedDamage(!isReptiliansAttack);
                }
            }
        }
        yield return new WaitForSeconds(Constants.TIME_DELAY);
    }

    private IEnumerator ProcessAbilities(int slotNumber, List<SlotBehaviour> attackerSlots, List<SlotBehaviour> defenderSlots)
    {
        CardBehaviour card = attackerSlots[slotNumber].Card;
        if (card.Data.ability != 0)
        {
            switch (card.Data.ability)
            {
                case CardData.Ability.UFO:
                    if (card.TurnsFromSpawn > 1 && !card.isUFOed)
                    {
                        yield return card.UFO();
                    };
                    break;
                case CardData.Ability.Tower:
                    if (slotNumber == 0)
                    {
                        if (attackerSlots[1].Card != null) yield return attackerSlots[1].Card.Aberrate();
                    }
                    else if (slotNumber == 3)
                    {
                        if (attackerSlots[2].Card != null) yield return attackerSlots[2].Card.Aberrate();
                    }
                    else
                    {
                        if (attackerSlots[slotNumber - 1].Card != null) yield return attackerSlots[slotNumber - 1].Card.Aberrate();
                        if (attackerSlots[slotNumber + 1].Card != null) yield return attackerSlots[slotNumber + 1].Card.Aberrate();
                    }
                    break;
                case CardData.Ability.FoilHat:
                    break;
                case CardData.Ability.Mimic:
                    if (defenderSlots[slotNumber].Card != null)
                    {
                        yield return card.Mimic(defenderSlots[slotNumber].Card.Data.attack, defenderSlots[slotNumber].Card.Data.health);
                    } else
                    {
                        if (card.Data.attack > 1 || card.Data.health > 1) yield return card.Mimic(1, 1);
                    }
                    break;
                case CardData.Ability.Regeneration:
                    yield return card.Regenerate();
                    break;
                case CardData.Ability.Betrayer:
                    if (playerHealth < 10f && !card.Data.isReptilian)
                    {
                        foreach(SlotBehaviour slot in defenderSlots)
                        {
                            if (slot.Card == null)
                            {
                                card.Lock();
                                card.Data.isReptilian = true;
                                attackerSlots[slotNumber].Card = null;
                                slot.Card = card;
                                yield return card.MoveAndRotateAnimation(slot.transform.position, Quaternion.identity, Constants.TIME_GET_CARD_ANIMATION);
                                break;
                            }
                        }
                    }
                    break;
            }
        }
    }

    private IEnumerator PlayerTurn(int numOfCards=1, bool firstTurn=false)
    {
        ShowMessage("YOUR TURN");
        yield return new WaitForSeconds(2 * Constants.TIME_FADE_ANIMATION + Constants.TIME_TEXT_DISPLAY_DURATION);
        if (firstTurn) StartCoroutine(AddCards(reptiliansHand, reptiliansDeck, 4));
        StartCoroutine(AddCards(playerHand, playerDeck, numOfCards));
        state = LevelStates.PlayerTurn;
        AddSpawnPoints(false);
        playerHand.UnlockCards();
        EventBus.Instance.RaiseOnTurnStarted();
    }

    private IEnumerator ReptiliansTurn(int numOfCards = 1, bool lastLevel=false)
    {
        ShowMessage("REPTILIANS TURN");
        yield return new WaitForSeconds(2 * Constants.TIME_FADE_ANIMATION + Constants.TIME_TEXT_DISPLAY_DURATION);
        if (lastLevel) StartCoroutine(AddCards(playerHand, playerDeck, 4));
        StartCoroutine(AddCards(reptiliansHand, reptiliansDeck, numOfCards));
        yield return new WaitForSeconds(numOfCards * Constants.TIME_GET_CARD_ANIMATION);
        state = LevelStates.ReptiliansTurn;
        AddSpawnPoints(true);
        playerHand.LockCards();
        EventBus.Instance.RaiseOnTurnStarted();

        AIController.MakeTurnDecision(reptiliansSpawnPointsUnused, reptiliansHand, reptiliansSlotBehaviours, playerSlotBehaviours, (AIController.Mode)aiMode);
        yield return new WaitForSeconds(Constants.TIME_DELAY);
        EventBus.Instance.RaiseOnTurnEnded();
    }

    private IEnumerator AddCards(HandManager hand, Stack<CardBehaviour> deck, int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            if (deck.Count > 0)
            {
                hand.AddCard(deck.Pop());
            }
            else
            {
                int health;
                if (data.level != 4) break;
                if (hand.isReptilian)
                {
                    reptiliansHealth -= 2;
                    health = reptiliansHealth;
                } else
                {
                    playerHealth -= 2;
                    health = playerHealth;
                }
                EventBus.Instance.RaiseOnOpponentHealthChanged(health, hand.isReptilian);
                EventBus.Instance.RaiseOnDeckIsEmpty(hand.isReptilian);
                break;
            }
            yield return new WaitForSeconds(Constants.TIME_GET_CARD_ANIMATION);
        }
    }

    private List<CardData> LoadDeckData(List<int> cardNumbers, List<CardData> allCards)
    {
        List<CardData> cardData = new();
        foreach (var number in cardNumbers)
        {
            CardData originalCardData = allCards[number];
            CardData copiedCardData = new CardData
            {
                title = originalCardData.title,
                description = originalCardData.description,
                cost = originalCardData.cost,
                attack = originalCardData.attack,
                health = originalCardData.health,
                avatarNumber = originalCardData.avatarNumber,
                isReptilian = originalCardData.isReptilian,
                ability = originalCardData.ability
            };

            cardData.Add(copiedCardData);
        }
        return cardData;
    }

    private Stack<CardBehaviour> LoadDeck(List<CardData> cardsData, Vector3 pos)
    {

        // sort by cost, then make a little mix
        cardsData.Sort((x, y) => y.cost.CompareTo(x.cost));
        int numOfCards = cardsData.Count;
        for (int i = 0; i < cardsData.Count / 3; i++)
        {
            Utility.Swap(cardsData, Random.Range(1, numOfCards / 2), Random.Range(numOfCards / 2, numOfCards));
        }

        // make a deck with cards
        GameObject card;
        CardBehaviour cardBehaviour;
        Stack<CardBehaviour> deck = new();
        foreach (CardData cardData in cardsData)
        {
            card = Instantiate(CardPrefab, pos, Quaternion.Euler(0, 0, 90 + Random.Range(-10, 10)));
            cardBehaviour = card.GetComponent<CardBehaviour>();
            cardBehaviour.LoadData(cardData);
            deck.Push(cardBehaviour);
        }
        return deck;
    }

    private List<SlotBehaviour> LoadSlots(GameObject slots)
    {
        List<SlotBehaviour> slotsBehaviours = new();
        if (slots != null)
        {
            slotsBehaviours.AddRange(slots.GetComponentsInChildren<SlotBehaviour>());
        }

        return slotsBehaviours;
    }

    private void RemoveCardFromHand(CardBehaviour card, bool isReptilian)
    {
        if (isReptilian)
        {
            reptiliansHand.RemoveCard(card);
        } else
        {
            if (data.level == 4) peaceCount -= 1000;
            playerHand.RemoveCard(card);
        }
    }

    private void AddSpawnPoints(bool isReptilian, int pointsToAdd = 1)
    {
        if (data.level == 4)
        {
            peaceCount += 1;
            if (peaceCount == 6)
            {
                SceneManager.LoadScene("GoodEnd");
            }
        }
        ref int spawnPointsRef = ref isReptilian ? ref reptiliansSpawnPoints : ref playerSpawnPoints;
        ref int spawnPointsUnusedRef = ref isReptilian ? ref reptiliansSpawnPointsUnused : ref playerSpawnPointsUnused;
        spawnPointsRef += pointsToAdd;
        spawnPointsRef = System.Math.Max(0, spawnPointsRef);
        spawnPointsRef = System.Math.Min(spawnPointsRef, Constants.MAX_SPAWN_POINTS);
        spawnPointsUnusedRef = spawnPointsRef;
        EventBus.Instance.RaiseOnSpawnPointsChanged(spawnPointsRef, isReptilian);
        EventBus.Instance.RaiseOnSpawnPointsUnusedChanged(spawnPointsUnusedRef, isReptilian);
    }

    private void UseSpawnPoints(CardBehaviour card, bool isReptilian)
    {
        ref int spawnPointsUnusedRef = ref isReptilian ? ref reptiliansSpawnPointsUnused : ref playerSpawnPointsUnused;
        spawnPointsUnusedRef -= card.Data.cost;
        EventBus.Instance.RaiseOnSpawnPointsUnusedChanged(spawnPointsUnusedRef, isReptilian);
    }

    private void ChangeHealth(int delta, bool isReptilian)
    {
        ref int health = ref isReptilian ? ref reptiliansHealth : ref playerHealth;
        health += delta;
        EventBus.Instance.RaiseOnOpponentHealthChanged(health, isReptilian);
        if (health <= 0) StartCoroutine(EndLevel());
    }

    private IEnumerator EndLevel()
    {
        if (playerHealth <= 0)
        {
            ShowMessage("YOU LOSE!");
            yield return new WaitForSeconds(Constants.TIME_FADE_ANIMATION + Constants.TIME_TEXT_DISPLAY_DURATION);
            GameManager.Instance.RestartLevel();
        }

        if (reptiliansHealth <= 0)
        {
            ShowMessage("YOU WIN!");
            yield return new WaitForSeconds(Constants.TIME_FADE_ANIMATION + Constants.TIME_TEXT_DISPLAY_DURATION);
            if (data.level == 4)
            {
                GameManager.Instance.GetComponentInChildren<AudioSource>().Stop();
                SceneManager.LoadScene("BadEnd");
            } else
            {
                GameManager.Instance.NextNews();
            }
        }
    }

    private void ShowMessage(string message)
    {
        EventBus.Instance.RaiseOnShowMessage(message);
    }

    private void ForceEndDragging()
    {
        if (Draggable.draggedNow != null)
        {
            Draggable.draggedNow.Lock();
            Draggable.draggedNow.ToStartPosition();
        }
    }

    public void OnTurnEndClick()
    {
        if (state == LevelStates.PlayerTurn) EventBus.Instance.RaiseOnTurnEnded();
    }
}
