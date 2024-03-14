using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandManager
{
    public readonly bool isReptilian;
    private readonly List<CardBehaviour> cards = new();
    private float cardWidth = 0f;
    private readonly float spaceBetweenCards;
    private readonly float posY;

    public HandManager(bool isReptilian)
    {
        this.isReptilian = isReptilian;
        spaceBetweenCards = Constants.MARGIN_BETWEEN_CARDS;
        posY = isReptilian ? Constants.POS_REPTILIANS_HAND_Y : Constants.POS_PLAYER_HAND_Y;
    }

    public void AddCard(CardBehaviour card)
    {
        if (cardWidth == 0) cardWidth = card.gameObject.GetComponent<SpriteRenderer>().size.x;

        // check is there enough space in hand
        if (cards.Count == Constants.MAX_CARDS_IN_HAND)
        {
            card.StartHandOverflowAnimation();
            return;
        };

        // add card
        cards.Add(card);
        int numberOfCards = cards.Count;
        float totalWidth = numberOfCards * cardWidth + (numberOfCards - 1) * spaceBetweenCards;
        float startX = -totalWidth / 2;

        // animate cards
        for (int i = 0; i < numberOfCards; i++)
        {
            float cardPosX = startX + i * (cardWidth + spaceBetweenCards);
            Vector3 newPosition = new(cardPosX, posY, 0);
            Quaternion newRotation;
            if (i == numberOfCards - 1)
            {
                newRotation = isReptilian ? Quaternion.Euler(0, 0, 180) : Quaternion.identity;
                if (!isReptilian) cards[i].BackView(false);
            } else
            {
                newRotation = cards[i].gameObject.transform.rotation;
            }
            cards[i].StartMoveAndRotateAnimation(newPosition, newRotation);
        }
        EventBus.Instance.RaiseOnNumberOfCardsInHandChanged(cards.Count, isReptilian);
    }

    public void RemoveCard(CardBehaviour card)
    {
        if (cards.Contains(card))
        {
            cards.Remove(card);
            float totalWidth = cards.Count * cardWidth + (cards.Count - 1) * spaceBetweenCards;
            float startX = -totalWidth / 2;

            for (int i = 0; i < cards.Count; i++)
            {
                float cardPosX = startX + i * (cardWidth + spaceBetweenCards);
                Vector3 newPosition = new(cardPosX, posY, 0);
                cards[i].StartMoveAndRotateAnimation(newPosition, cards[i].transform.rotation);
            }
        }
        EventBus.Instance.RaiseOnNumberOfCardsInHandChanged(cards.Count, isReptilian);
    }

    public void PlaceCardToSlot(CardBehaviour card, SlotBehaviour slot)
    {
        if (!cards.Contains(card))
        {
            return; // card not in hand
        }

        RemoveCard(card); // remove the card from hand first

        slot.Card = card;
        card.BackView(false);
        card.StartMoveAndRotateAnimation(slot.transform.position, Quaternion.identity);

        EventBus.Instance.RaiseOnNumberOfCardsInHandChanged(cards.Count, isReptilian);
    }

    public List<CardBehaviour> GetCards()
    {
        return new List<CardBehaviour>(cards);
    }

    public void LockCards()
    {
        foreach (CardBehaviour card in cards)
        {
            card.Lock();
        }
    }

    public void UnlockCards()
    {
        foreach (CardBehaviour card in cards)
        {
            card.Unlock();
        }
    }
}
