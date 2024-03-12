using System;

namespace LostCities.GameModel
{
    public class DrawPile : CardPile
    {
        public const int NUM_WAGER_CARDS_PER_EXPEDITION = 3;
        public const int NUM_CHECKPOINT_CARDS_PER_EXPEDITION = 9;
        public const int NUM_CARDS_PER_EXPEDITION = NUM_WAGER_CARDS_PER_EXPEDITION + NUM_CHECKPOINT_CARDS_PER_EXPEDITION;

        public DrawPile() : base()
        {
            Label = "Draw Pile";

            // Populate the pile with all of the expedition cards
            for (int e = 0; e < (int)Expedition.COUNT; e++)
            {
                Expedition x = (Expedition)e;
                // Add WAGER cards
                for (int w = 0; w < NUM_WAGER_CARDS_PER_EXPEDITION; w++)
                {
                    cards.Add(new ExpeditionCard { Expedition = x, Value = ExpeditionCard.WAGER_VALUE });
                }
                // Add CHECKPOINT cards
                for (int checkpoint = ExpeditionCard.MIN_CHECKPOINT_VALUE; checkpoint <= ExpeditionCard.MAX_CHECKPOINT_VALUE; checkpoint++)
                {
                    cards.Add(new ExpeditionCard { Expedition = x, Value = checkpoint });
                }
            }
        }

        public void Shuffle()
        {
            Random rng = new((int)DateTime.Now.Ticks);

            // Shuffle the pile (Fisher–Yates / Durstenfeld shuffle algorithm)
            for (int cardIndex = 0; cardIndex < cards.Count - 1; cardIndex++)
            {
                // Pick a random card between this card and the last card in the pile
                int otherIndex = rng.Next(cardIndex, cards.Count);
                // Swap cards
                (cards[cardIndex], cards[otherIndex]) = (cards[otherIndex], cards[cardIndex]);
            }
        }
    }
}
