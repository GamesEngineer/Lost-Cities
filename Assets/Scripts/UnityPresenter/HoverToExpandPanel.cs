using UnityEngine;
using UnityEngine.EventSystems;

public class HoverToExpand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Vector2 sizeWhenClosed = new (320f, 50f);
    [SerializeField] Vector2 sizeWhenOpened = new (1000f, 700f);

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.sizeDelta = sizeWhenOpened;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.sizeDelta = sizeWhenClosed;
    }

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = transform as RectTransform;
        rectTransform.sizeDelta = sizeWhenClosed;
    }
}
