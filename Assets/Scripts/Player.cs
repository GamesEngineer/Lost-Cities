using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerHand Hand { get; private set; }
    protected DrawPile drawPile;
    protected DiscardPile[] discardPiles;

    private void Awake()
    {
        Hand = transform.GetChild(0).GetComponent<PlayerHand>();
        drawPile = FindObjectOfType<DrawPile>();
        discardPiles = FindObjectsOfType<DiscardPile>();
    }

    protected virtual void OnEnable()
    {
        DrawPile.OnClicked += HandleDrawPileClicked;
        DiscardPile.OnClicked += HandleDiscardPileClicked;
    }

    protected virtual void OnDisable()
    {
        DrawPile.OnClicked -= HandleDrawPileClicked;
        DiscardPile.OnClicked -= HandleDiscardPileClicked;
    }

    private void Start()
    {
        SetPilesAsSelectable(false);
    }

    public Card RemoveSelectedCard() => Hand.RemoveCard(Hand.SelectedCard.card);

    public virtual IEnumerator DrawCard()
    {
        int numCards = Hand.Cards.Count;
        SetPilesAsSelectable(true);
        yield return new WaitUntil(() => Hand.Cards.Count >= 8);
        yield return null; // ensure the card animation starts to play before turning "isSelectable" off
        SetPilesAsSelectable(false);
        yield return new WaitForSecondsRealtime(1.5f);
        Hand.DeselectHoverCard();
    }

    private void SetPilesAsSelectable(bool arePilesSelectable)
    {
        drawPile.IsPlayerSelectable = arePilesSelectable;
        foreach (var discardPile in discardPiles)
        {
            discardPile.IsPlayerSelectable = arePilesSelectable;
        }
    }

    private void HandleDrawPileClicked(DrawPile pile)
    {
        if (pile.IsPlayerSelectable)
        {
            var topCard = pile.RemoveCardFromTop();
            Hand.AddCard(topCard);
        }
    }

    private void HandleDiscardPileClicked(DiscardPile pile)
    {
        if (pile.IsPlayerSelectable)
        {
            var topCard = pile.RemoveCardFromTop();
            Hand.AddCard(topCard);
        }
    }
}
