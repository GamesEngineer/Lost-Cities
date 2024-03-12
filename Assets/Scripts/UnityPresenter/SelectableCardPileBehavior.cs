using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LostCities.UnityPresenter
{
    public class SelectableCardPileBehavior : MonoBehaviour
    {
        public static event Action<SelectableCardPileBehavior, PointerEventData> OnClicked;
        public GameModel.CardPile CardPile { get; set; } // set by the RefereeBehavior

        public virtual bool IsPlayerSelectable
        {
            get => visibleCard.IsPlayerSelectable;
            set => visibleCard.IsPlayerSelectable = value;
        }

        private void Start()
        {
            visibleCard = GetComponentInChildren<ExpeditionCardBehavior>();
            visibleCard.OnClicked += VisibleCard_OnClicked;
        }

        protected virtual void Update()
        {
            visibleCard.Card = CardPile.Cards.Count > 0 ? CardPile.Cards[^1] : GameModel.ExpeditionCard.Invalid;
        }

        private void VisibleCard_OnClicked(ExpeditionCardBehavior _, PointerEventData pointerEventData)
        {
            OnClicked?.Invoke(this, pointerEventData);
        }

        protected ExpeditionCardBehavior visibleCard;
    }
}
