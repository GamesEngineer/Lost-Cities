namespace LostCities.GameModel
{
    public class DiscardPile : CardPile
    {
        public Expedition Expedition 
        {
            get => _expedition;
            set 
            {
                if (_expedition == value) return;
                _expedition = value;
                Label = $"{_expedition} Discard Pile";
            }
        }
        private Expedition _expedition = Expedition.INVALID;
    }
}
