using System;
using UnityEngine;

public class EventBus : MonoBehaviour
{
    public static EventBus Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // called when a card is placed in a slot
    public Action<CardBehaviour, bool> OnCardPlaced;
    public void RaiseOnCardPlaced(CardBehaviour card, bool isReptilian)
    {
        OnCardPlaced?.Invoke(card, isReptilian);
    }

    // called when start of turn occurs
    public Action OnTurnStarted;
    public void RaiseOnTurnStarted()
    {
        OnTurnStarted?.Invoke();
    }

    // called when end of turn occurs
    public Action OnTurnEnded;
    public void RaiseOnTurnEnded()
    {
        OnTurnEnded?.Invoke();
    }

    // called when hand becomes full and new card is destroyed
    public Action<bool> OnHandOverflow;
    public void RaiseOnHandOverflow(bool isReptilian)
    {
        OnHandOverflow?.Invoke(isReptilian);
    }

    // called when trying to draw a card from an empty deck
    public Action<bool> OnOutOfCards;
    public void RaiseOnDeckIsEmpty(bool isReptilian)
    {
        OnOutOfCards?.Invoke(isReptilian);
    }

    // called when trying to spawn a card with not enogh spawn points
    public Action<bool> OnOutOfSpawnPoints;
    public void RaiseOnOutOfSpawnPoints(bool isReptilian)
    {
        OnOutOfSpawnPoints?.Invoke(isReptilian);
    }

    // called every new turn to change the number of spawn points
    public Action<int, bool> OnSpawnPointsChanged;
    public void RaiseOnSpawnPointsChanged(int points, bool isReptilian)
    {
        OnSpawnPointsChanged?.Invoke(points, isReptilian);
    }

    // called when unused points increase with a new turn or decrease when the card spawns
    public Action<int, bool> OnSpawnPointsUnusedChanged;
    public void RaiseOnSpawnPointsUnusedChanged(int points, bool isReptilian)
    {
        OnSpawnPointsUnusedChanged?.Invoke(points, isReptilian);
    }

    // called when the number of cards in hand is changed
    public Action<int, bool> OnNumberOfCardsInHandChanged;
    public void RaiseOnNumberOfCardsInHandChanged(int points, bool isReptilian)
    {
        OnNumberOfCardsInHandChanged?.Invoke(points, isReptilian);
    }

    // called when battle between cards started
    public Action OnBattleStarted;
    public void RaiseOnBattleStarted()
    {
        OnBattleStarted?.Invoke();
    }

    // called when player or reptilians boss received damage
    public Action<bool> OnOpponentReceivedDamage;
    public void RaiseOnOpponentReceivedDamage(bool isReptilian)
    {
        OnOpponentReceivedDamage?.Invoke(isReptilian);
    }

    // called when player's or reptilians boss' health changed 
    public Action<int, bool> OnOpponentHealthChanged;
    public void RaiseOnOpponentHealthChanged(int health, bool isReptilian)
    {
        OnOpponentHealthChanged?.Invoke(health, isReptilian);
    }

    // called when need to fade screen in
    public Action OnFadeIn;
    public void RaiseOnFadeIn()
    {
        OnFadeIn?.Invoke();
    }

    // called when need to fade screen out
    public Action OnFadeOut;
    public void RaiseOnFadeOut()
    {
        OnFadeOut?.Invoke();
    }

    // called when need to show message
    public Action<string> OnShowMessage;
    public void RaiseOnShowMessage(string message)
    {
        OnShowMessage?.Invoke(message);
    }

    public Action<Vector3> OnUFO;
    public void RaiseOnUFO(Vector3 position)
    {
        OnUFO?.Invoke(position);
    }

    public Action<string, bool> OnNameChanged;
    public void RaiseOnNameChanged(string message, bool isReptilian)
    {
        OnNameChanged?.Invoke(message, isReptilian);
    }

    public Action<int, bool> OnBossAvatarChanged;
    public void RaiseOnBossAvatarChanged(int avatarNumber, bool isReptilian)
    {
        OnBossAvatarChanged?.Invoke(avatarNumber, isReptilian);
    }
}
