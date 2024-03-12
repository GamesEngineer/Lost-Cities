using UnityEngine;

namespace LostCities.UnityPresenter
{
    public abstract class PlayerBehavior : MonoBehaviour
    {
        public GameModel.Player PlayerModel { get; set; } // set by RefereeBehavior

        public PlayerHandBehavior HandObj { get; private set; } // set in Awake
        public ExpeditionPileBehavior[] ExpeditionPileObjs { get; private set; } // set in Awake

        protected virtual void Awake()
        {
            HandObj = GetComponentInChildren<PlayerHandBehavior>(includeInactive: true);
            ExpeditionPileObjs = GetComponentsInChildren<ExpeditionPileBehavior>(includeInactive: true);
        }

        protected abstract void OnEnable();

        protected abstract void OnDisable();
    }
}
