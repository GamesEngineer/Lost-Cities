namespace LostCities.UnityPresenter
{
    public class DiscardPileBehavior : SelectableCardPileBehavior
    {
        public GameModel.Expedition Expedition => (CardPile as GameModel.DiscardPile).Expedition;

        protected override void Update()
        {
            visibleCard.Card = CardPile.Cards.Count > 0 ? CardPile.Cards[^1] : GameModel.ExpeditionCard.Placeholders[(int)Expedition];
        }
    }
}
