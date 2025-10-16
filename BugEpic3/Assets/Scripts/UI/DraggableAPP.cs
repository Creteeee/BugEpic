using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class DraggableApp : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private WindowsGridSystem gridSystem;
    
    [Header("拖动设置")]
    public float dragSpeed = 1f;
    public bool snapToGrid = true;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gridSystem = FindObjectOfType<WindowsGridSystem>();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta * dragSpeed;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (snapToGrid && gridSystem != null)
        {
            Vector2 worldPosition = rectTransform.position;
            Vector2 nearestCenter = gridSystem.GetNearestGridCenter(worldPosition);
            rectTransform.position = nearestCenter;
        }
    }
}