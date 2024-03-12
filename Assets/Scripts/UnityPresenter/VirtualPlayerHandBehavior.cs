using System.Collections;
using UnityEngine;

namespace LostCities.UnityPresenter
{
    public class VirtualPlayerHandBehavior : PlayerHandBehavior
    {
        public override IEnumerator RaiseHand()            
        {
            Hand.Owner.CurrentState = GameModel.Player.State.GETTING_READY_FOR_TURN;
            while (Vector3.Distance(transform.localPosition, raisedPosition) > Time.deltaTime * 1.5f)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, raisedPosition, Time.deltaTime * 3f);
                yield return null;
            }
            transform.localPosition = raisedPosition;
            yield return new WaitForSecondsRealtime(0.5f);
            Hand.Owner.CurrentState = GameModel.Player.State.TAKING_TURN;
        }

        public override IEnumerator SelectOneCard()
        {
            // Selection is actually done by the GameModel
            yield return null;
        }
    }
}
