using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Card))]
public class DiscardPile : MonoBehaviour, ICardPile, IPointerClickHandler
{
    public Card.Expedition expedition;
    public Card TopCard { get; private set; }
    private readonly List<Card.Data> cards = new List<Card.Data>();
    public IReadOnlyList<Card.Data> Cards => cards;
    public static event Action<DiscardPile> OnClicked;
    private Card visibleCard;

    public bool IsPlayerSelectable
    {
        get => visibleCard.IsPlayerSelectable;
        set => visibleCard.IsPlayerSelectable = value;
    }

    private void Awake()
    {
        visibleCard = GetComponent<Card>();
        visibleCard.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Pointer CLICKED discard pile {expedition}");
        OnClicked?.Invoke(this);
    }

    public void AddCardToTop(Card.Data card)
    {
        Assert.IsTrue(card.expedition == expedition);
        cards.Add(card);
        visibleCard.data = card;
        visibleCard.gameObject.SetActive(true);
    }

    public Card.Data RemoveCardFromTop()
    {
        int topCardIndex = cards.Count - 1;
        var topCard = cards[topCardIndex];
        cards.RemoveAt(topCardIndex);
        if (cards.Count == 0)
        {
            visibleCard.gameObject.SetActive(false);
            TopCard = null;
        }
        else
        {
            topCardIndex = cards.Count - 1;
            visibleCard.data = cards[topCardIndex];
        }
        return topCard;
    }
}
