using UnityEngine;
using UnityEngine.UI;

namespace LostCities.UnityPresenter
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class ExpeditionPileBehavior : MonoBehaviour
    {
        [SerializeField] ExpeditionCardBehavior facecardPrefab;

        public GameModel.ExpeditionPile ExpeditionPile // set by the RefereeBehavior
        {
            get => _expeditionPile;
            set 
            {
                if (value == _expeditionPile) return;
                if (_expeditionPile is not null)
                {
                    _expeditionPile.OnAddCardToTop -= ExpeditionPile_OnAddCardToTop;
                }
                _expeditionPile = value;
                if (_expeditionPile is not null)
                {
                    _expeditionPile.OnAddCardToTop += ExpeditionPile_OnAddCardToTop;
                }
            }
        }
        private GameModel.ExpeditionPile _expeditionPile;

        private void Awake()
        {
            // Pre-populate all of the game objects that will represent cards 
            for (int i = 0; i < cardObjects.Length; i++)
            {
                ExpeditionCardBehavior cardObj = Instantiate(facecardPrefab, transform);
                cardObj.Card = new GameModel.ExpeditionCard()
                {
                    Expedition = ExpeditionPile?.Expedition ?? GameModel.Expedition.WHITE,
                    Value = GameModel.ExpeditionCard.INVALID_VALUE
                };
                cardObjects[i] = cardObj;
            }
        }

        private void ExpeditionPile_OnAddCardToTop(GameModel.ExpeditionCard previousTopCard)
        {
            int topCardIndex = ExpeditionPile.Cards.Count - 1;
            cardObjects[topCardIndex].Card = ExpeditionPile.TopCard;
        }

        private readonly ExpeditionCardBehavior[] cardObjects = new ExpeditionCardBehavior[GameModel.DrawPile.NUM_CARDS_PER_EXPEDITION];
    }
}
