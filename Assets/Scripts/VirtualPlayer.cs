using System.Collections;
using UnityEngine;

public class VirtualPlayer : Player
{
    protected override void OnEnable() { }
    protected override void OnDisable() { }

    public override IEnumerator DrawCard()
    {
        Card.Data? topCard = null;

        // CHALLENGE: improve the pile selection with "smarter" heuristics
        for (int i = 0; i < (int)Card.Expedition.COUNT; i++)
        {
            var discardPile = Referee.Instance.discardPiles[i];
            var expeditionPile = Referee.Instance.opponentExpeditionPiles[i];
            if (expeditionPile.TopCard != null && discardPile.TopCard != null && 
                discardPile.TopCard.data.value == (expeditionPile.TopCard.data.value + 1))
            {
                topCard = discardPile.RemoveCardFromTop();
                break;
            }
        }

        topCard = topCard ?? drawPile.RemoveCardFromTop();
        Hand.AddCard(topCard.Value);
        yield return new WaitForSecondsRealtime(1f);

        Hand.DeselectHoverCard();
    }
}
