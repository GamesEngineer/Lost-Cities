using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum Expedition { WHITE, YELLOW, BLUE, GREEN, RED, /**/ COUNT };

    [System.Serializable]
    public struct Data
    {
        public int value;
        public Expedition expedition;
        public string Label => $"{expedition}_{value}";
    }

    public Data data;
    public readonly Color[] colors = { Color.white, Color.yellow, Color.blue, Color.green, Color.red };
    public Image artImage;
    public TextMeshProUGUI valueText;
    public bool IsHighlighted { get; set; }

    void Update()
    {
        if (artImage && valueText)
        {
            artImage.color = colors[(int)data.expedition];
            valueText.text = data.value == 0 ? "X" : data.value.ToString();
            valueText.color = artImage.color;
        }
        name = data.Label;
    }

    public event Action<Card, PointerEventData> OnClicked;
    public event Action<Card, PointerEventData> OnHover;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Pointer CLICKED card {data.Label}");
        OnClicked?.Invoke(this, eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover?.Invoke(this, eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHover?.Invoke(this, eventData);
    }
}
