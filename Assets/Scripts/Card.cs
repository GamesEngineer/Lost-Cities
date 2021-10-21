using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum Expedition { WHITE, YELLOW, BLUE, GREEN, RED, /**/ COUNT };
    public Sprite[] expeditionArt;

    private Animator anim;

    public bool IsPlayerSelectable
    {
        get => anim.GetBool("isSelectable");
        set => anim.SetBool("isSelectable", value);
    }

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

    private void OnValidate()
    {
        if (artImage && valueText)
        {
            artImage.sprite = expeditionArt[(int)data.expedition];
            artImage.color = Color.Lerp(colors[(int)data.expedition], Color.white, 0.6f);
            valueText.text = data.value == 0 ? "X" : data.value.ToString();
            valueText.color = artImage.color;
        }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (artImage && valueText)
        {
            artImage.sprite = expeditionArt[(int)data.expedition];
            artImage.color = Color.Lerp(colors[(int)data.expedition], Color.white, 0.6f);
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
        anim.SetTrigger("select");
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
