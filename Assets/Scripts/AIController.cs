using System.Collections.Generic;
using System.Linq;

public class AIController
{
    public enum Mode
    {
        Defence = 1,
        Attack = 2,
        Balance = 3
    }

    public static void MakeTurnDecision(int spawnPoints, HandManager reptiliansHand, List<SlotBehaviour> reptiliansSlots, List<SlotBehaviour> playerSlots, Mode mode)
    {
        switch (mode)
        {
            case Mode.Defence:
                Defend(spawnPoints, reptiliansHand, reptiliansSlots, playerSlots);
                break;
            case Mode.Attack:
                Attack(spawnPoints, reptiliansHand, reptiliansSlots, playerSlots);
                break;
            case Mode.Balance:
                int enemyThreatLevel = playerSlots.Count(slot => slot.Card != null);
                int ourDefenseLevel = reptiliansSlots.Count(slot => slot.Card != null);
                if (enemyThreatLevel > ourDefenseLevel)
                {
                    Defend(spawnPoints, reptiliansHand, reptiliansSlots, playerSlots);
                }
                else
                {
                    Attack(spawnPoints, reptiliansHand, reptiliansSlots, playerSlots);
                }
                break;
        }
    }


    // method to defend by placing cards directly opposite to enemy cards first, then filling remaining slots
    private static void Defend(int spawnPoints, HandManager reptiliansHand, List<SlotBehaviour> ourSlots, List<SlotBehaviour> enemySlots)
    {
        // first, we protect slots directly opposite to enemy cards
        for (int i = 0; i < enemySlots.Count; i++)
        {
            if ((enemySlots[i].Card != null) && (ourSlots.Count > i && ourSlots[i].Card == null))
            {
                var cardToPlay = reptiliansHand.GetCards()
                    .FirstOrDefault(cb => cb.Data.cost <= spawnPoints && cb.Data.health > enemySlots[i].Card.Data.attack);

                if (cardToPlay == null) // if we don't find a perfect match, use the cheapest card available
                {
                    cardToPlay = reptiliansHand.GetCards()
                        .FirstOrDefault(cb => cb.Data.cost <= spawnPoints);
                }

                if (cardToPlay != null)
                {
                    reptiliansHand.PlaceCardToSlot(cardToPlay, ourSlots[i]); // place the card in the slot
                    spawnPoints -= cardToPlay.Data.cost;
                    if (spawnPoints < 0) break; // ensure we don't go negative
                }
            }
        }

        // if we still have spawn points, fill the remaining slots
        FillRemainingSlots(spawnPoints, reptiliansHand, ourSlots);
    }

    // method to attack by placing cards opposite empty enemy slots first, then filling remaining slots
    private static void Attack(int spawnPoints, HandManager reptiliansHand, List<SlotBehaviour> ourSlots, List<SlotBehaviour> enemySlots)
    {
        // first, target empty slots on the enemy side
        for (int i = 0; i < enemySlots.Count; i++)
        {
            if (enemySlots[i].Card == null && ourSlots.Count > i && ourSlots[i].Card == null)
            {
                var cardToPlay = reptiliansHand.GetCards()
                    .FirstOrDefault(cb => cb.Data.cost <= spawnPoints);

                if (cardToPlay != null)
                {
                    reptiliansHand.PlaceCardToSlot(cardToPlay, ourSlots[i]); // place the card in the slot
                    spawnPoints -= cardToPlay.Data.cost;
                    if (spawnPoints < 0) break; // ensure we don't go negative
                }
            }
        }

        // then, fill remaining slots with whatever spawn points are left
        FillRemainingSlots(spawnPoints, reptiliansHand, ourSlots);
    }



    // helper method to fill remaining slots
    private static void FillRemainingSlots(int spawnPoints, HandManager reptiliansHand, List<SlotBehaviour> ourSlots)
    {
        foreach (var card in reptiliansHand.GetCards())
        {
            if (spawnPoints <= 0) break; // stop if we run out of spawn points

            if (card.Data.cost <= spawnPoints)
            {
                for (int i = 0; i < ourSlots.Count; i++)
                {
                    if (ourSlots[i].CanAcceptCard(card.Data) && ourSlots[i].Card == null)
                    {
                        reptiliansHand.PlaceCardToSlot(card, ourSlots[i]); // place the card in the slot
                        spawnPoints -= card.Data.cost;
                        break; // move on to the next card once one is used
                    }
                }
            }
        }
    }
}
