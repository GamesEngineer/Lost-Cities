using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostCities.GameModel
{
    public enum IllegalMove { PLAYED_CARD_TOO_SMALL, TOOK_CARD_THAT_WAS_JUST_DISCARDED }

    public class Referee
    {
        public const int MIN_ROUTE_LENGTH_FOR_BONUS = 8;
        public const int INVESTMENT_COST = 20;

        public event Action OnReset;
        public event Action OnStartGame;
        public event Action<Player> OnTurnStarted;
        public event Action<Player> OnCardSelected;
        public event Action<Player> OnActionSelected;
        public event Action<Player, IllegalMove> OnIllegalMove;
        public event Action<Player> OnCardPlaced;
        public event Action<Player> OnHandRefilled;
        public event Action<Player> OnTurnEnded;
        public event Action OnGameOver;

        // The Commons
        public DrawPile CommonDrawPile { get; private set; }
        public DiscardPile[] CommonDiscardPiles { get; private set; }

        // The Players
        public LocalPlayer LocalPlayer { get; private set; }
        public VirtualPlayer OpponentPlayer { get; private set; }
        public int[/*player turn order*/] CurrentScore { get; private set; } = new int[2];

        public bool IsGameOver => CommonDrawPile.Cards.Count <= 0;

        public void Reset()
        {
            LocalPlayer = new LocalPlayer(turnOrder: 0, this);
            OpponentPlayer = new VirtualPlayer(turnOrder: 1, this);
            CommonDrawPile = new DrawPile();
            CommonDrawPile.Shuffle();
            CommonDiscardPiles = new DiscardPile[(int)Expedition.COUNT];

            // Ensure that all of the expeditions are represented in the same order
            for (int e = 0; e < (int)Expedition.COUNT; e++)
            {
                Expedition expedition = (Expedition)e;
                CommonDiscardPiles[e] = new();
                CommonDiscardPiles[e].Expedition = expedition;
                LocalPlayer.ExpeditionPiles[e].Expedition = expedition;
                OpponentPlayer.ExpeditionPiles[e].Expedition = expedition;
            }

            OnReset?.Invoke();
        }

        public void StartGame()
        {
            // Both players start the game by drawing a hand of cards from the draw pile
            for (int i = 0; i < PlayerHand.MAX_HAND_SIZE; i++)
            {
                LocalPlayer.TakeTopCardFromPile(CommonDrawPile);
                OpponentPlayer.TakeTopCardFromPile(CommonDrawPile);
            }

            OnStartGame?.Invoke();
        }

        public IEnumerator DoGameLoop()
        {
            while (true) // DoPlayerTurn will exit the coroutine when the game is over
            {
                yield return DoPlayerTurn(LocalPlayer);
                yield return DoPlayerTurn(OpponentPlayer);
            }
        }

        private IEnumerator DoPlayerTurn(Player player)
        {
            if (IsGameOver) yield break;

            OnTurnStarted?.Invoke(player);

            yield return player.StartTurnAndWaitUntilReady();

            // SELECT a card from the player's hand, and then do either:
            //   1) PLAY the selected card onto its expedition pile
            //   2) DISCARD the selected card onto its discard pile
            bool isPlacementValid = false;
            do
            {
                yield return player.WaitUntilCardIsSelectedFromHand();
                OnCardSelected?.Invoke(player);

                yield return player.WaitUntilCardActionIsSelected(); // PLAY or DISCARD
                OnActionSelected?.Invoke(player);

                CardPile targetPile = GetTargetPile(player);
                isPlacementValid = IsCardPlayableOnTargetPile(player, targetPile);
                if (isPlacementValid)
                {
                    player.PlaceSelectedCardOnPile(targetPile);
                    OnCardPlaced?.Invoke(player);
                }
                else
                {
                    OnIllegalMove?.Invoke(player, IllegalMove.PLAYED_CARD_TOO_SMALL);
                }
            } while (!isPlacementValid);

            CurrentScore[player.TurnOrder] = CalculateTotalScore(player);

            // DRAW a new card from the top of the draw pile OR any discard pile,
            // and put it in the player's hand.
            yield return player.WaitUntilCardTakenFromCommons(CommonDrawPile, CommonDiscardPiles);

            OnHandRefilled?.Invoke(player);

            yield return new WaitForSecondsRealtime(1.0f);

            yield return player.WaitUntilTurnFinished();

            OnTurnEnded?.Invoke(player);

            if (IsGameOver)
            {
                OnGameOver?.Invoke();
                yield break;
            }
        }

        private CardPile GetTargetPile(Player player)
        {
            CardPile pile;
            int e = (int)player.SelectedCard.Expedition;
            pile = player.CardAction switch
            {
                CardAction.PLAY => player.ExpeditionPiles[e],
                CardAction.DISCARD => CommonDiscardPiles[e],
                _ => default
            };
            return pile;
        }

        private static bool IsCardPlayableOnTargetPile(Player player, CardPile pile)
        {
            return player.CardAction == CardAction.DISCARD ||
                player.CardAction == CardAction.PLAY && player.SelectedCard.Value >= pile.TopCard.Value;
        }

        public static int CalculateTotalScore(Player player)
        {
            int totalScore = 0;

            foreach (var e in player.ExpeditionPiles)
            {
                int expeditionScore = CalculateExpeditionScore(e.Cards);
                totalScore += expeditionScore;
            }

            return totalScore;
        }

        public static int CalculateExpeditionScore(IReadOnlyCollection<ExpeditionCard> cards)
        {
            // Inactive expeditions do not affect the score
            if (cards.Count == 0) return 0;

            // Active expeditions always start with an initial investment
            int expeditionScore = -INVESTMENT_COST;

            // Score the expedition's activity
            int wagerMultiplier = 1;
            foreach (var c in cards)
            {
                Debug.Assert(c.IsValid);
                if (c.IsWager) wagerMultiplier++;
                else expeditionScore += c.Value;
            }
            expeditionScore *= wagerMultiplier;

            // Expeditions with enough activity recoup their intial investment
            // NOTE: This happens AFTER the investment multiplier.
            // This means that expeditions with poor performance can get hammered by unsuccessful wagers.
            if (cards.Count >= MIN_ROUTE_LENGTH_FOR_BONUS)
            {
                expeditionScore += INVESTMENT_COST;
            }

            return expeditionScore;
        }
    }
}
