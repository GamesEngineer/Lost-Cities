using System.Collections;

namespace LostCities.GameModel
{
    public class LocalPlayer : Player
    {
        public LocalPlayer(int turnOrder, Referee referee) : base(turnOrder, referee) {}

        public override IEnumerator StartTurnAndWaitUntilReady()
        {
            SelectedCard = ExpeditionCard.Invalid;
            CardAction = CardAction.INVALID;
            while (CurrentState == State.GETTING_READY_FOR_TURN)
                yield return null;
        }

        public override IEnumerator WaitUntilCardIsSelectedFromHand()
        {
            while (!Hand.Owner.SelectedCard.IsValid)
                yield return null;
        }

        public override IEnumerator WaitUntilCardActionIsSelected()
        {
            while (CardAction == CardAction.INVALID)
                yield return null;
        }

        public override IEnumerator WaitUntilCardTakenFromCommons(DrawPile drawPile, DiscardPile[] discardPiles)
        {
            while (TargetCardPile is null)
                yield return null;
            TakeTopCardFromPile(TargetCardPile);
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
