using System.Collections;

namespace LostCities.GameModel
{
    public enum CardAction { INVALID = -1, PLAY, DISCARD }

    public abstract class Player
    {
        public Player(int turnOrder, Referee referee)
        {
            TurnOrder = turnOrder;
            Referee = referee;
            Hand = new PlayerHand(this);             
            ExpeditionPiles = new ExpeditionPile[(int)Expedition.COUNT];
            for (int e = 0; e < (int)Expedition.COUNT; e++)
            {
                ExpeditionPiles[e] = new();
            }
        }

        public int TurnOrder { get; private set; }
        public Referee Referee { get; private set; }
        public PlayerHand Hand { get; private set; }
        public ExpeditionPile[] ExpeditionPiles { get; private set; } // Length = Expedition.COUNT
        public ExpeditionCard SelectedCard { get; set; }
        public CardAction CardAction { get; set; } = CardAction.INVALID;
        public CardPile TargetCardPile { get; set; } // either the DrawPile or one of the DiscardPiles
        public enum State { WAITING, GETTING_READY_FOR_TURN, TAKING_TURN, FINISHING_TURN };
        public State CurrentState { get; set; }

        public abstract IEnumerator StartTurnAndWaitUntilReady();

        public abstract IEnumerator WaitUntilCardIsSelectedFromHand();

        public abstract IEnumerator WaitUntilCardActionIsSelected();

        public void PlaceSelectedCardOnPile(CardPile pile)
        {
            ExpeditionCard card = Hand.RemoveCard(Hand.Owner.SelectedCard);
            pile.AddCardToTop(card);
        }

        public abstract IEnumerator WaitUntilCardTakenFromCommons(DrawPile drawPile, DiscardPile[] discardPiles);

        public void TakeTopCardFromPile(CardPile pile)
        {
            ExpeditionCard card = pile.RemoveCardFromTop();
            MostRecentCardDraw = new CardDraw(card, pile);
            Hand.AddCard(card);
        }

        public abstract IEnumerator WaitUntilTurnFinished();

        public readonly struct CardDraw
        {
            public readonly ExpeditionCard card;
            public readonly CardPile pile;
            public CardDraw(ExpeditionCard card, CardPile pile)
            {
                this.card = card;
                this.pile = pile;
            }
        }

        public CardDraw MostRecentCardDraw { get; private set; }
    }
}
