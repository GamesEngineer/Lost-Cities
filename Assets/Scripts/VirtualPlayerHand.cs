using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualPlayerHand : PlayerHand
{
    public override IEnumerator SelectOneCard()
    {
        // Clear any previous selection
        SelectedCard = null;

        DiscardUnplayableCard();

        if (SelectedCard == null)
        {
            SelectNextCard();
        }

        Debug.Log($"AI selected {SelectedCard.card.Label} with {SelectedCard.selectionButton}");

        yield return new WaitForSecondsRealtime(0.1f);
    }

    private Card DiscardUnplayableCard()
    {
        foreach (var card in Cards)
        {
            var pile = Referee.Instance.opponentExpeditionPiles[(int)card.data.expedition];
            if (pile.TopCard != null && card.data.value < pile.TopCard.data.value)
            {
                SelectedCard = new Selection
                {
                    card = card.data,
                    selectionButton = PointerEventData.InputButton.Right,
                };
                return card;
            }
        }

        return null;
    }

    private Card SelectNextCard()
    {
        Card bestCard = null;
        foreach (var card in Cards)
        {
            int x = (int)card.data.expedition;
            if (bestCard == null || card.data.value < bestCard.data.value) { bestCard = card; }
        }

        SelectedCard = new Selection
        {
            card = bestCard.data,
            selectionButton = PointerEventData.InputButton.Left,
        };

        return bestCard;
    }
}
