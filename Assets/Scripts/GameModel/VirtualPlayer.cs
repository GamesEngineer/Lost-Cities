using System.Collections;

namespace LostCities.GameModel
{
    public class VirtualPlayer : Player
    {
        public VirtualPlayer(int turnOrder, Referee referee) : base(turnOrder, referee) {}

        public override IEnumerator StartTurnAndWaitUntilReady()
        {
            SelectedCard = ExpeditionCard.Invalid;
            CardAction = CardAction.INVALID;
            while (CurrentState == State.GETTING_READY_FOR_TURN)
                yield return null;
        }

        public override IEnumerator WaitUntilCardIsSelectedFromHand()
        {
            // CHALLENGE - make this AI algorithm be smarter

            int bestCardToPlay = -1;
            int bestCardToDiscard = -1;
            int lowestPlayableCardValue = int.MaxValue;
            int highestDiscardableCardValue = -1;
            for (int i = 0; i < Hand.Cards.Count; i++)
            {
                ExpeditionCard card = Hand.Cards[i];
                int e = (int)card.Expedition;
                int checkpoint = ExpeditionPiles[e].TopCard.Value;
                if (card.Value < lowestPlayableCardValue && card.Value > checkpoint)
                {
                    lowestPlayableCardValue = card.Value;
                    bestCardToPlay = i;
                }
                else if (card.Value > highestDiscardableCardValue)
                {
                    highestDiscardableCardValue = card.Value;
                    bestCardToDiscard = i;
                }
            }
            
            if (bestCardToPlay > -1)
            {
                SelectedCard = Hand.GetCard(bestCardToPlay);
                CardAction = CardAction.PLAY;
            }
            else
            {
                SelectedCard = Hand.GetCard(bestCardToDiscard);
                CardAction = CardAction.DISCARD;
            }

            yield return null;
        }

        public override IEnumerator WaitUntilCardActionIsSelected()
        {
            // CardAction should have already been selected in WaitUntilCardIsSelectedFromHand()
            while (CardAction == CardAction.INVALID)
                yield return null;
        }

        public override IEnumerator WaitUntilCardTakenFromCommons(DrawPile drawPile, DiscardPile[] discardPiles)
        {
            TargetCardPile = drawPile; // default to the common draw pile, unless we can find a good discard pile

            // CHALLENGE: improve the pile selection with "smarter" heuristics
            for (int e = 0; e < (int)Expedition.COUNT; e++)
            {
                var discardPile = discardPiles[e];
                var expeditionPile = ExpeditionPiles[e];
                if (discardPile.TopCard.Value == (expeditionPile.TopCard.Value + 1) ||
                    discardPile.TopCard.Value == (expeditionPile.TopCard.Value + 2))
                {
                    TargetCardPile = discardPile;                    
                    break;
                }
            }

            yield return null;
            TakeTopCardFromPile(drawPile);
            TargetCardPile = null;
        }

        public override IEnumerator WaitUntilTurnFinished()
        {
            SelectedCard = ExpeditionCard.Invalid;
            CardAction = CardAction.INVALID;
            while (CurrentState == State.FINISHING_TURN)
                yield return null;
        }
    }
}
