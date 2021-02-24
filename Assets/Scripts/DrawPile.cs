using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawPile : MonoBehaviour, ICardPile, IPointerClickHandler
{
    public TextMeshProUGUI cardsRemainingText;
    private readonly List<Card.Data> cards = new List<Card.Data>();
    public IReadOnlyList<Card.Data> Cards => cards;
    public int CardsRemaining => cards.Count;

    private void Awake() // NOTE: 1/26/2020 - changed from Start to Awake so that Referee.Start has a valid draw pile to use
    {
        // Populate the pile with all of the expedition cards
        for (var x = Card.Expedition.WHITE; x <= Card.Expedition.RED; x++)
        {
            // Add three INVESTMENT cards per expedition
            cards.Add(new Card.Data { expedition = x, value = 0 });
            cards.Add(new Card.Data { expedition = x, value = 0 });
            cards.Add(new Card.Data { expedition = x, value = 0 });
            // Add ten CHECKPOINT cards per expedition
            for (int v = 1; v <= 10; v++)
            {
                cards.Add(new Card.Data { expedition = x, value = v });
            }
        }

        // Shuffle the pile
        for (int cardIndex = 0; cardIndex < cards.Count; cardIndex++)
        {
            // Pick a random card between this card and the last card in the pile
            int otherIndex = UnityEngine.Random.Range(cardIndex, cards.Count);
            // Swap this card with the other card
            var otherCard = cards[otherIndex];
            cards[otherIndex] = cards[cardIndex];
            cards[cardIndex] = otherCard;
        }
    }

    private void Update()
    {
        cardsRemainingText.text = $"{cards.Count} Remaining";
    }

    public static event Action<DrawPile> OnClicked;

    public void OnPointerClick(PointerEventData _)
    {
        OnClicked?.Invoke(this);
    }

    public void AddCardToTop(Card.Data card)
    {
        cards.Add(card);
    }

    public Card.Data RemoveCardFromTop()
    {
        int topCardIndex = cards.Count - 1;
        var topCard = cards[topCardIndex];
        cards.RemoveAt(topCardIndex);
        return topCard;
    }

    public Card TopCard => throw new NotImplementedException();
}
