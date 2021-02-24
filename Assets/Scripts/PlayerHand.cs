#define KEEP_HAND_SORTED
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerHand : MonoBehaviour
{
    public Card cardPrefab;
    private readonly List<Card> cards = new List<Card>();
    public IReadOnlyList<Card> Cards => cards;

#if KEEP_HAND_SORTED
    private class CardComparer : IComparer<Card>
    {
        public int Compare(Card x, Card y)
        {
            //return string.CompareOrdinal(x.data.Label, y.data.Label);
            int xOrder = x.data.value + (int)x.data.expedition * 11;
            int yOrder = y.data.value + (int)y.data.expedition * 11;
            return xOrder - yOrder;
        }
    }

    private readonly CardComparer cardComparer = new CardComparer();
#endif

    public Card AddCard(Card.Data cardData)
    {
        var cardObj = Instantiate<Card>(cardPrefab, transform);
        cardObj.data = cardData;
#if KEEP_HAND_SORTED
        int index = cards.BinarySearch(cardObj, cardComparer);
        if (index < 0) index = ~index;
        cards.Insert(index, cardObj);
        cardObj.transform.SetSiblingIndex(index);
#else
        cards.Add(cardObj);
#endif
        return cardObj;
    }
    public Card RemoveCard(int index)
    {
        Card cardObj = null;
        if (index >= 0 && index < cards.Count)
        {
            cardObj = cards[index];
            cards.RemoveAt(index);
        }
        return cardObj;
    }
    public Card RemoveCard(Card.Data cardData)
    {
        int index = cards.FindIndex( (card) => card.data.Label == cardData.Label );
        return RemoveCard(index);
    }
    public class Selection
    {
        public Card.Data card;
        public PointerEventData.InputButton selectionButton;
    }
    public Selection SelectedCard { get; protected set; }

    // TODO  - 3/02/2021 
    public virtual IEnumerator SelectOneCard()
    {
        // Clear any previous selection
        SelectedCard = null;

        // Register event handlers on this hand's cards
        foreach (var card in Cards)
        {
            card.OnClicked += HandleCardClicked;
            card.OnHover += HandleCardHover;
        }

        // Wait for player to select a card from this hand
        yield return new WaitUntil( () => SelectedCard != null );

        // De-register event handlers on this hand's cards
        foreach (var card in Cards)
        {
            card.OnClicked -= HandleCardClicked;
            card.OnHover -= HandleCardHover;
        }
    }
    private void HandleCardClicked(Card card, PointerEventData eventData)
    {
        // Only accept button clicks from LEFT or RIGHT mouse buttons
        if (eventData.button != PointerEventData.InputButton.Left &&
            eventData.button != PointerEventData.InputButton.Right) return;

        SelectedCard = new Selection { card = card.data, selectionButton = eventData.button };
        Debug.Log($"Selected {card.data.Label} with {eventData.button}");
    }

    private Card hoverCard;

    private void HandleCardHover(Card card, PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.isValid)
        {
            if (hoverCard != null)
            {
                hoverCard.IsHighlighted = false;
            }
            hoverCard = card;
            card.IsHighlighted = true;
        }
        else
        {
            if (hoverCard == card)
            {
                hoverCard = null;
            }
            card.IsHighlighted = false;
        }
    }

    public IEnumerator RaiseTo(float height)
    {
        while (transform.position.y < height)
        {
            transform.Translate(Vector3.up * 3f * Time.deltaTime, Space.Self);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator LowerTo(float height)
    {
        while (transform.position.y > height)
        {
            transform.Translate(Vector3.down * 3f * Time.deltaTime, Space.Self);
            yield return new WaitForEndOfFrame();
        }
    }

    private void Update()
    {
        RaiseHighlightedCard();
    }

    protected void RaiseHighlightedCard()
    {
        foreach (var card in cards)
        {
            var pos = card.transform.localPosition;
            if (card == hoverCard)
            {
                pos.y = (pos.y < 0.5f) ? (pos.y + Time.deltaTime * 3f) : 0.5f;
            }
            else
            {
                pos.y = (pos.y > 0f) ? (pos.y - Time.deltaTime * 5f) : 0f;
            }
            card.transform.localPosition = pos;
        }
    }
}
