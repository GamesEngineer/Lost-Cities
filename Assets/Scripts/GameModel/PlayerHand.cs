using System;
using System.Collections.Generic;

namespace LostCities.GameModel
{
    public class PlayerHand
    {
        public const int MAX_HAND_SIZE = 8;

        public IReadOnlyList<ExpeditionCard> Cards => cards;

        public Player Owner { get; private set; }

        public event Action<ExpeditionCard, int/*index*/> OnCardAdded;
        public event Action<ExpeditionCard, int/*index*/> OnCardRemoved;

        public PlayerHand(Player owner)
        {
            Owner = owner;
        }

        public ExpeditionCard GetCard(int index)
        {
            bool isValidIndex = index >= 0 && index < cards.Count;
            return isValidIndex ? cards[index] : ExpeditionCard.Invalid;
        }

        public void AddCard(ExpeditionCard card)
        {
            int index = cards.BinarySearch(card, cardComparer);
            if (index < 0)
            {
                // In this case the, index is the bitwise complement of the correct insertion index to keep things sorted.
                // https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.binarysearch?view=net-8.0
                index = ~index;
            }
            cards.Insert(index, card);
            OnCardAdded?.Invoke(card, index);
        }

        public ExpeditionCard RemoveCard(int index)
        {
            ExpeditionCard card = cards[index];
            cards.RemoveAt(index);
            OnCardRemoved?.Invoke(card, index);
            return card;
        }

        public ExpeditionCard RemoveCard(ExpeditionCard cardData)
        {
            int index = cards.FindIndex((card) => card.Label == cardData.Label);
            return RemoveCard(index);
        }

        protected readonly List<ExpeditionCard> cards = new(MAX_HAND_SIZE);
        private readonly CardComparer cardComparer = new();

        private class CardComparer : IComparer<ExpeditionCard>
        {
            const int NUM_UNIQUE_CARD_VALUES_PER_EXPEDITION = 10; // 1 WAGER card type + 9 CHECKPOINT card types

            public int Compare(ExpeditionCard x, ExpeditionCard y)
            {
                int xOrder = x.Value + (int)x.Expedition * NUM_UNIQUE_CARD_VALUES_PER_EXPEDITION;
                int yOrder = y.Value + (int)y.Expedition * NUM_UNIQUE_CARD_VALUES_PER_EXPEDITION;
                return xOrder - yOrder;
            }
        }

    }
}
