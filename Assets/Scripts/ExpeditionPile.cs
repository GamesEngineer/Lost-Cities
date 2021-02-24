using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ExpeditionPile : MonoBehaviour, ICardPile
{
    public Card.Expedition expedition;

    private readonly List<Card.Data> cards = new List<Card.Data>();
    private Card[] cardObjects;

    public IReadOnlyList<Card.Data> Cards => cards;
    public Card TopCard => cards.Count > 0 ? cardObjects[cards.Count - 1] : null;

    private void Awake()
    {
        cardObjects = GetComponentsInChildren<Card>();
    }

    public void Update()
    {
        foreach (var card in cardObjects)
        {
            card.gameObject.SetActive(card.data.value >= 0 && card.data.value <= 10);
        }
    }

    public void AddCardToTop(Card.Data cardData)
    {
        Assert.IsTrue(cardData.expedition == expedition);
        cardObjects[cards.Count].data = cardData;
        cards.Add(cardData);
    }

    public Card.Data RemoveCardFromTop()
    {
        int topCardIndex = cards.Count - 1;
        var topCard = cards[topCardIndex];
        cardObjects[cards.Count].data = new Card.Data(); // BUG? FIXME?
        cards.RemoveAt(topCardIndex);
        return topCard;
    }
}
