using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LostCities.GameModel
{
    public abstract class CardPile
    {
        public IReadOnlyList<ExpeditionCard> Cards => cards;

        public string Label { get; protected set; } = "Abstract Card Pile";

        public void AddCardToTop(ExpeditionCard card)
        {
            Debug.Assert(card.IsValid);
            ExpeditionCard previousTopCard = cards.Count > 0 ? cards[^1] : ExpeditionCard.Invalid;
            cards.Add(card);
            OnAddCardToTop?.Invoke(previousTopCard);
        }

        public ExpeditionCard RemoveCardFromTop()
        {
            int topCardIndex = cards.Count - 1;
            ExpeditionCard topCard = cards[topCardIndex];
            cards.RemoveAt(topCardIndex);
            OnRemoveCardFromTop?.Invoke(topCard);
            return topCard;
        }

        public ExpeditionCard TopCard
        {
            get
            {
                int topCardIndex = cards.Count - 1;
                ExpeditionCard topCard = topCardIndex >= 0 ? cards[topCardIndex] : ExpeditionCard.Invalid;
                return topCard;
            }
        }

        public event Action<ExpeditionCard /*previousTopCard*/> OnAddCardToTop;
        public event Action<ExpeditionCard /*previousTopCard*/> OnRemoveCardFromTop;

        protected readonly List<ExpeditionCard> cards = new();
    }
}
