using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerHand Hand { get; private set; }
    protected DrawPile drawPile;

    private void Awake()
    {
        Hand = transform.GetChild(0).GetComponent<PlayerHand>();
        drawPile = FindObjectOfType<DrawPile>();
    }

    public Card RemoveSelectedCard() => Hand.RemoveCard(Hand.SelectedCard.card);

    public virtual IEnumerator DrawCard()
    {
        int numCards = Hand.Cards.Count;
        DrawPile.OnClicked += HandleDrawPileClicked;
        DiscardPile.OnClicked += HandleDiscardPileClicked;
        yield return new WaitUntil(() => Hand.Cards.Count == numCards + 1);
        DrawPile.OnClicked -= HandleDrawPileClicked;
        DiscardPile.OnClicked -= HandleDiscardPileClicked;
    }

    private void HandleDrawPileClicked(DrawPile pile)
    {
        var topCard = pile.RemoveCardFromTop();
        Hand.AddCard(topCard);
    }

    private void HandleDiscardPileClicked(DiscardPile pile)
    {
        var topCard = pile.RemoveCardFromTop();
        Hand.AddCard(topCard);
    }
}
