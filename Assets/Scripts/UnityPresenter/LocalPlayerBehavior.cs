using UnityEngine.EventSystems;

namespace LostCities.UnityPresenter
{
    public class LocalPlayerBehavior : PlayerBehavior
    {
        protected override void OnEnable()
        {
            SelectableCardPileBehavior.OnClicked += TargetTheCardPile;
        }

        protected override void OnDisable()
        {
            SelectableCardPileBehavior.OnClicked -= TargetTheCardPile;
        }

        private void TargetTheCardPile(SelectableCardPileBehavior pile, PointerEventData pointerEventData)
        {
            if (pile.IsPlayerSelectable && pointerEventData.button == PointerEventData.InputButton.Left)
            {
                PlayerModel.TargetCardPile = pile.CardPile;
            }
        }
    }
}
