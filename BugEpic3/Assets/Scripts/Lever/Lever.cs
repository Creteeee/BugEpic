using UnityEngine;
using UnityEngine.EventSystems;

public class Lever : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public Transform leftEnd;    
    public Transform rightEnd;   
    public Transform pivotPoint; 
    public float dragAreaRadius ;  
    
    public float maxRotationAngle = 20f;  
    
    private LeverManager manager;
    private bool isDragging = false;
    private bool isSnapped = false;
    private bool isRotating = false;      
    private float initialZRotation;      
    private Vector2 initialLocalPos;      


    private void Awake()
    {
        initialZRotation = transform.eulerAngles.z;
        initialLocalPos = transform.localPosition;
    }

    public void Initialize(LeverManager leverManager)
    {
        manager = leverManager;
        
        if (leftEnd == null) Debug.LogError("请设置杠杆左端点！");
        if (rightEnd == null) Debug.LogError("请设置杠杆右端点！");
        if (pivotPoint == null) Debug.LogError("请设置旋转支点！");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!manager.IsSystemActive) return;

        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out worldPoint);
        
        if (!isSnapped)
        {
            float distanceToLeft = Vector2.Distance(leftEnd.position, worldPoint);
            if (distanceToLeft <= dragAreaRadius)
            {
                isDragging = true;
                isRotating = false;
            }
        }
        else
        {
            float distanceToRight = Vector2.Distance(rightEnd.position, worldPoint);
            if (distanceToRight <= dragAreaRadius)
            {
                isDragging = true;
                isRotating = true;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || !manager.IsSystemActive) return;

        Vector3 worldPoint;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out worldPoint))
        {
            if (!isSnapped && !isRotating)
            {
                Vector2 direction = (Vector2)worldPoint - (Vector2)leftEnd.position;
                transform.position += (Vector3)direction;
            }
            else if (isSnapped && isRotating && pivotPoint != null)
            {
                Vector2 pivotPos = pivotPoint.position;
                Vector2 mouseDir = (Vector2)worldPoint - pivotPos;
                float targetAngle = Mathf.Atan2(mouseDir.y, mouseDir.x) * Mathf.Rad2Deg;
                float angleDifference = Mathf.DeltaAngle(initialZRotation, targetAngle);
                angleDifference = Mathf.Clamp(angleDifference, -maxRotationAngle, 0);
                float finalAngle = initialZRotation + angleDifference;
                transform.eulerAngles = new Vector3(0, 0, finalAngle);
                
                manager.OnLeverRotated(angleDifference); 
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        isRotating = false;

        if (!manager.IsSystemActive) return;
        
        if (!isSnapped && manager.CheckIfShouldSnap(transform.position))
        {
            SnapToPosition();
        }
    }

    // 吸附到目标位置
    public void SnapToPosition()
    {
        transform.position = manager.targetPosition.position;
        isSnapped = true;
        // 记录吸附时的初始旋转角度
        initialZRotation = transform.eulerAngles.z;
        manager.OnLeverSnapped();
        Debug.Log("杠杆已吸附到目标位置");
    }

    // 重置杠杆状态
    public void ResetLever()
    {
        isSnapped = false;
        isDragging = false;
        isRotating = false;
        transform.localPosition = initialLocalPos;
        transform.eulerAngles = new Vector3(0, 0, initialZRotation);
        manager.OnLeverReset();
    }
    
    
}
