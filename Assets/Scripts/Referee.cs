using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class Referee : MonoBehaviour
{
    public DrawPile drawPile;
    public DiscardPile[] discardPiles;
    public ExpeditionPile[] playerExpeditionPiles;
    public ExpeditionPile[] opponentExpeditionPiles;
    public Player localPlayer;
    public VirtualPlayer opponentPlayer;
    public TextMeshProUGUI gameEndText;
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI opponentScoreText;

    public static Referee Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        gameEndText.gameObject.SetActive(false);

        // Both players automatically draw a hand of 8 cards from the draw pile
        for (int i = 0; i < 8; i++)
        {
            Card.Data card;

            card = drawPile.RemoveCardFromTop();
            localPlayer.Hand.AddCard(card);

            card = drawPile.RemoveCardFromTop();
            opponentPlayer.Hand.AddCard(card);
        }

        // Ensure the expeditions are represented
        for (int e = 0; e < (int)Card.Expedition.COUNT; e++)
        {
            discardPiles[e].TopCard.data.expedition = (Card.Expedition)e;
            playerExpeditionPiles[e].expedition = (Card.Expedition)e;
            opponentExpeditionPiles[e].expedition = (Card.Expedition)e;
        }

        localPlayer.Hand.DeselectHoverCard();
        opponentPlayer.Hand.DeselectHoverCard();

        UpdateScoreText(localPlayer.name, playerScoreText, playerExpeditionPiles);
        UpdateScoreText(opponentPlayer.name, opponentScoreText, opponentExpeditionPiles);

        StartCoroutine(DoPlayerTurns());
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private IEnumerator DoPlayerTurns()
    {
        yield return opponentPlayer.Hand.LowerTo(-1f);

        while (true)
        {
            yield return DoTurn(localPlayer, playerExpeditionPiles, discardPiles, playerScoreText);
            yield return CheckForGameEnd();

            yield return DoTurn(opponentPlayer, opponentExpeditionPiles, discardPiles, opponentScoreText);
            yield return CheckForGameEnd();
        }
    }

    private static IEnumerator DoTurn(Player player, ExpeditionPile[] expeditionPiles, DiscardPile[] discardPiles, TextMeshProUGUI scoreText)
    {
        yield return player.Hand.RaiseTo(0.75f);

        // Play a card from the player's hand onto an expedition or onto its color's discard pile
        ICardPile pile = null;
        while (pile == null)
        {
            yield return player.Hand.SelectOneCard();

            var cardData = player.Hand.SelectedCard.card;
            switch (player.Hand.SelectedCard.selectionButton)
            {
                case PointerEventData.InputButton.Left: // PLAY the card
                    {
                        pile = expeditionPiles[(int)cardData.expedition];
                        if (pile.TopCard != null && cardData.value < pile.TopCard.data.value)
                        {
                            pile = null; // illegal move; try again
                        }
                    }
                    break;
                case PointerEventData.InputButton.Right: // DISCARD the card
                    {
                        pile = discardPiles[(int)cardData.expedition];
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        };

        // Remove the selected card from the player's hand,
        // and place the card onto the appropriate pile.
        Card cardObj = player.RemoveSelectedCard();
        pile.AddCardToTop(cardObj.data);
        Destroy(cardObj.gameObject);

        UpdateScoreText(player.name, scoreText, expeditionPiles);

        // Draw a new card from the top of the draw pile OR any discard pile
        yield return player.DrawCard();

        yield return new WaitForSecondsRealtime(1f);

        yield return player.Hand.LowerTo(-1f);
    }

    private static void UpdateScoreText(string playerName, TextMeshProUGUI scoreText, ExpeditionPile[] expeditionPiles)
    {
        int totalScore = CalculateTotalScore(expeditionPiles);
        scoreText.text = $"{playerName} Score: {totalScore}";
    }

    public static int CalculateTotalScore(ExpeditionPile[] expeditionPiles)
    {
        int totalScore = 0;

        foreach (var e in expeditionPiles)
        {
            int expeditionScore = CalculateExpeditionScore(e.Cards);
            totalScore += expeditionScore;
        }

        return totalScore;
    }

    public static int CalculateExpeditionScore(IReadOnlyCollection<Card.Data> cards)
    {
        // Inactive expeditions do not affect the score
        if (cards.Count == 0) return 0;

        // Active expeditions start with an initial investment of -20
        int expeditionScore = -20;

        // Score the expedition's activity
        int multiplier = 1;
        foreach (var c in cards)
        {
            if (c.value == 0) multiplier++;
            else expeditionScore += c.value;
        }
        expeditionScore *= multiplier;

        // Expeditions with enough activity recoup their intial investment (AFTER the investment multiplier)
        if (cards.Count >= 8) expeditionScore += 20;

        return expeditionScore;
    }

    private IEnumerator CheckForGameEnd()
    {
        // Check if the game has ended
        if (drawPile.CardsRemaining == 0)
        {
            Debug.Log("GAME OVER");
            // TODO - Tally the score and display the winner
            gameEndText.gameObject.SetActive(true);

            // Reload the game after a few seconds
            yield return new WaitForSecondsRealtime(5f);

            // TODO - present retry and quit options
            SceneManager.LoadScene(0);
        }
    }

}
