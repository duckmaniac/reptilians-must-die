using UnityEngine;

public class SlotBehaviour : MonoBehaviour
{
    [SerializeField] bool isReptilian;
    public CardBehaviour Card { get; set; }
    private int unusedSpawnPoints = 0;

    private void Awake()
    {
        GetComponent<CircleCollider2D>().enabled = !isReptilian;
    }

    private void OnEnable()
    {
        EventBus.Instance.OnSpawnPointsUnusedChanged += SetUnusedSpawnPoints;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnSpawnPointsUnusedChanged -= SetUnusedSpawnPoints;
    }

    public bool CanAcceptCard(CardData cardData)
    {
        if (Card != null) return false;
        if (cardData.cost > unusedSpawnPoints)
        {
            EventBus.Instance.RaiseOnOutOfSpawnPoints(isReptilian);
            return false;
        }
        return true;
    }

    private void SetUnusedSpawnPoints(int points, bool isReptilian)
    {
        if (this.isReptilian == isReptilian) unusedSpawnPoints = points;
    }
}
