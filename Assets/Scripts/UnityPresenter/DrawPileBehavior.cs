using TMPro;
using UnityEngine;

namespace LostCities.UnityPresenter
{
    public class DrawPileBehavior : SelectableCardPileBehavior
    {
        [SerializeField] TextMeshProUGUI cardsRemainingText;

        protected override void Update()
        {
            base.Update();
            cardsRemainingText.text = $"{CardPile.Cards.Count} Cards Remaining";
        }
    }
}
