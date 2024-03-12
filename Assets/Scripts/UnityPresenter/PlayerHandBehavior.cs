using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace LostCities.UnityPresenter
{
    public class PlayerHandBehavior : MonoBehaviour
    {
        [SerializeField] protected ExpeditionCardBehavior cardPrefab;
        [SerializeField] protected Vector3 loweredPosition;

        /// <summary>
        /// Setting a Hand also registers it with its referee's events:
        /// OnTurnStarted, OnTurnEnded
        /// </summary>
        public GameModel.PlayerHand Hand // set by RefereeBehavior
        {
            get => _hand;
            set
            {
                if (_hand == value) return;
                if (_hand is not null)
                {
                    _hand.OnCardAdded -= Hand_OnCardAdded;
                    _hand.Owner.Referee.OnTurnStarted -= RefereeModel_OnTurnStarted;
                    _hand.Owner.Referee.OnTurnEnded -= RefereeModel_OnTurnEnded;
                }
                _hand = value;
                if (_hand is not null)
                {
                    _hand.OnCardAdded += Hand_OnCardAdded;
                    _hand.Owner.Referee.OnTurnStarted += RefereeModel_OnTurnStarted;
                    _hand.Owner.Referee.OnTurnEnded += RefereeModel_OnTurnEnded;
                }
            }
        }
        private GameModel.PlayerHand _hand;

        public IReadOnlyCollection<ExpeditionCardBehavior> CardObjs => cardObjects;

        private void RefereeModel_OnTurnStarted(GameModel.Player player)
        {
            if (player != Hand.Owner) return;

            Assert.IsFalse(Hand.Owner.SelectedCard.IsValid);
            StartCoroutine(RaiseHand());
        }

        private void RefereeModel_OnTurnEnded(GameModel.Player player)
        {
            if (player != Hand.Owner) return;

            StartCoroutine(LowerHand());
        }

        public ExpeditionCardBehavior RemoveCard(int cardIndex)
        {
            ExpeditionCardBehavior cardObj = cardObjects[cardIndex];
            Hand.RemoveCard(cardObj.Card);
            return cardObj;
        }

        public virtual IEnumerator SelectOneCard()
        {
            // Register event handlers on this hand's cards
            foreach (var cardObj in cardObjects)
            {
                cardObj.OnClicked += HandleCardSelected;
            }

            // Wait for player to select a card from this hand
            yield return new WaitUntil(() => Hand.Owner.SelectedCard.IsValid);

            // De-register event handlers on this hand's cards
            foreach (var cardObj in cardObjects)
            {
                cardObj.OnClicked -= HandleCardSelected;
            }
        }

        private void HandleCardSelected(ExpeditionCardBehavior cardObj, PointerEventData eventData)
        {
            // Only accept button clicks from LEFT or RIGHT mouse buttons
            if (eventData.button != PointerEventData.InputButton.Left &&
                eventData.button != PointerEventData.InputButton.Right) return;

            Hand.Owner.SelectedCard = Hand.GetCard(Array.IndexOf(cardObjects, cardObj));
            Hand.Owner.CardAction = eventData.button switch
            {
                PointerEventData.InputButton.Left => GameModel.CardAction.PLAY,
                PointerEventData.InputButton.Right => GameModel.CardAction.DISCARD,
                _ => GameModel.CardAction.INVALID
            };
            Debug.Log($"LocalPlayer chose to {Hand.Owner.CardAction} {Hand.Owner.SelectedCard.Label}");
        }

        public virtual IEnumerator RaiseHand()
        {
            float delta = RAISE_SPEED * Time.deltaTime;
            Hand.Owner.CurrentState = GameModel.Player.State.GETTING_READY_FOR_TURN;
            while (Vector3.Distance(transform.localPosition, raisedPosition) > delta / 2f)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, raisedPosition, delta);
                yield return null;
            }
            transform.localPosition = raisedPosition;
            Hand.Owner.CurrentState = GameModel.Player.State.TAKING_TURN;

            // Enable card selection, then wait for a card to be selected, then disable card selection
            yield return SelectOneCard();
        }

        public virtual IEnumerator LowerHand()
        {
            float delta = LOWER_SPEED * Time.deltaTime;
            Hand.Owner.CurrentState = GameModel.Player.State.TAKING_TURN;
            while (Vector3.Distance(transform.localPosition, loweredPosition) > delta / 2f)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, loweredPosition, delta);
                yield return null;
            }
            transform.localPosition = loweredPosition;
            Hand.Owner.CurrentState = GameModel.Player.State.WAITING;
        }

        private void Hand_OnCardAdded(GameModel.ExpeditionCard card, int cardIndex)
        {
            for (int i = 0; i < GameModel.PlayerHand.MAX_HAND_SIZE; i++)
            {
                cardObjects[i].Card = Hand.GetCard(i);                
            }

            // Make the card appear to drop into the hand by starting it as a hover card
            refillCardIndex = cardIndex;
        }        

        private void Awake()
        {
            // Pre-populate all of the game objects that will represent cards in the player's hand
            for (int i = 0; i < GameModel.PlayerHand.MAX_HAND_SIZE; i++)
            {
                cardObjects[i] = Instantiate(cardPrefab, transform);
            }

            raisedPosition = transform.localPosition;
            transform.localPosition = loweredPosition;
        }

        private void Update()
        {
            if (Hand is null) return;

            float maxRaiseDelta = RAISE_SPEED * Time.deltaTime;
            float maxLowerDelta = LOWER_SPEED * Time.deltaTime;

            for (int i = 0; i < GameModel.PlayerHand.MAX_HAND_SIZE; i++)
            {
                var cardObj = cardObjects[i];
                cardObj.Card = Hand.GetCard(i);
                RaiseOrLower(maxRaiseDelta, maxLowerDelta, cardObj);
            }

            if (refillCardIndex != -1)
            {
                ExpeditionCardBehavior cardObj = cardObjects[refillCardIndex];
                var pos = cardObj.transform.localPosition;
                pos.y = 0.75f;
                cardObj.transform.localPosition = pos;
                refillCardIndex = -1;
            }
        }

        private void RaiseOrLower(float maxRaiseDelta, float maxLowerDelta, ExpeditionCardBehavior cardObj)
        {
            var pos = cardObj.transform.localPosition;
            
            if (cardObj.IsHighlighted)
            {
                pos.y = Mathf.MoveTowards(pos.y, 0.5f, maxRaiseDelta);
            }
            else
            {
                pos.y = Mathf.MoveTowards(pos.y, 0f, maxLowerDelta);
            }
            cardObj.transform.localPosition = pos;
        }

        protected readonly ExpeditionCardBehavior[] cardObjects = new ExpeditionCardBehavior[GameModel.PlayerHand.MAX_HAND_SIZE];
        protected Vector3 raisedPosition;
        protected int refillCardIndex = -1;
        protected const float RAISE_SPEED = 3f;
        protected const float LOWER_SPEED = 5f;
    }
}