using System.Collections.Generic;

public interface ICardPile
{
    IReadOnlyList<Card.Data> Cards { get; }
    void AddCardToTop(Card.Data card);
    Card.Data RemoveCardFromTop();
    Card TopCard { get; }
}
