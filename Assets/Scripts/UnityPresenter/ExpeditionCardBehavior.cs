using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LostCities.UnityPresenter
{
    [ExecuteAlways]
    public class ExpeditionCardBehavior : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public static Destinations destinations;
        [SerializeField] Image artImage;
        [SerializeField] TextMeshProUGUI valueText;
        [SerializeField] bool showFrontFace;
        [SerializeField] Sprite backFaceArt;
        [SerializeField] GameModel.Expedition placeholderExpedition;

        public GameModel.ExpeditionCard Card 
        {
            get => _cardModel;
            set
            {
                if (_cardModel.Label == value.Label) return;
                _cardModel = value;
                Update();
            }
        }
        GameModel.ExpeditionCard _cardModel;

        public bool IsPlayerSelectable // TODO - FIXME! This doesn't work well with the player's hand management (i.e., pooled card objects)
        {
            get { return Card.IsValid && (anim ? anim.GetBool("isSelectable") : false); }
            set { if (anim) anim.SetBool("isSelectable", value && Card.IsValid); }
        }

        /// <summary>
        /// Returns true when the mouse pointer is hovering over this card.
        /// </summary>
        public bool IsHighlighted { get; private set; }

        public event Action<ExpeditionCardBehavior, PointerEventData> OnClicked;
        public event Action<ExpeditionCardBehavior, PointerEventData> OnPointerEntered;
        public event Action<ExpeditionCardBehavior, PointerEventData> OnPointerExited;

        private void Awake()
        {
            panelImage = GetComponent<Image>();
            anim = GetComponent<Animator>();
        }

        private void OnValidate()
        {
            Card = (placeholderExpedition >= 0 && placeholderExpedition < GameModel.Expedition.COUNT)
                ? GameModel.ExpeditionCard.Placeholders[(int)placeholderExpedition]
                : GameModel.ExpeditionCard.Invalid;
        }

        private void Update()
        {
            if (!destinations)
            {
                destinations = Resources.Load<Destinations>("Destinations");
            }

            if (!panelImage)
            {
                panelImage = GetComponent<Image>();
            }

            if (Application.isPlaying) // prefabs names must match their filename, so don't change the name outside of play mode
            {
                name = Card.Label;
            }

            bool show = Card.IsValid || Card.IsPlaceholder || !Application.isPlaying;
            
            panelImage.enabled = show;
            artImage.enabled = show;
            valueText.enabled = show;
            if (!show) return;

            if (artImage)
            {
                int e = (int)Card.Expedition;
                if (e < 0 || e >= (int)GameModel.Expedition.COUNT) e = 0;
                if (showFrontFace)
                {
                    artImage.sprite = destinations.cardArtwork[e];
                    artImage.color = Color.Lerp(Destinations.colors[e], Color.white, 0.6f);
                }
                else
                {
                    artImage.sprite = backFaceArt;
                    artImage.color = Color.white;
                }
            }

            if (valueText)
            {
                if (showFrontFace)
                {
                    valueText.text = Card.ValueLabel;
                    valueText.color = artImage.color;
                }
                else
                {
                    valueText.text = "LOST CITIES";
                    valueText.color = Color.white;
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"Pointer CLICKED card {name}");
            anim.SetTrigger("select");
            OnClicked?.Invoke(this, eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"Pointer ENTERED {name}");
            IsHighlighted = true;
            OnPointerEntered?.Invoke(this, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log($"Pointer EXITED {name}");
            IsHighlighted = false;
            OnPointerExited?.Invoke(this, eventData);
        }

        private Image panelImage;
        private Animator anim;
    }
}
