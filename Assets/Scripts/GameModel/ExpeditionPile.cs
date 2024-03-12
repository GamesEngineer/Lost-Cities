namespace LostCities.GameModel
{
    public class ExpeditionPile : CardPile
    {
        public const int MAX_CARDS_IN_EXPEDITION = 13; // 3 WAGER cards + 10 CHECKPOINT cards

        public Expedition Expedition { get; set; }
    }
}
