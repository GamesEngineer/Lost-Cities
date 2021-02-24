using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Card))]
public class DiscardPile : MonoBehaviour, ICardPile, IPointerClickHandler
{
    public Card TopCard { get; private set; }
    private readonly List<Card.Data> cards = new List<Card.Data>();
    public IReadOnlyList<Card.Data> Cards => cards;
    public static event Action<DiscardPile> OnClicked;

    private void Awake()
    {
        TopCard = GetComponent<Card>();
        TopCard.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke(this);
    }

    public void AddCardToTop(Card.Data card)
    {
        Assert.IsTrue(card.expedition == TopCard.data.expedition);
        cards.Add(card);
        TopCard.data = card;
        TopCard.gameObject.SetActive(true);
    }

    public Card.Data RemoveCardFromTop()
    {
        int topCardIndex = cards.Count - 1;
        var topCard = cards[topCardIndex];
        cards.RemoveAt(topCardIndex);
        if (cards.Count == 0)
        {
            TopCard.gameObject.SetActive(false);
        }
        else
        {
            topCardIndex = cards.Count - 1;
            TopCard.data = cards[topCardIndex];
        }
        return topCard;
    }
}
