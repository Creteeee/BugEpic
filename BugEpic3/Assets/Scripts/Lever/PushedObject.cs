using UnityEngine;

public class PushedObject : MonoBehaviour
{
    [Header("旋转设置")]
    public Transform rotatePivot; 
    private Vector2 initialOffset; 
    private float initialRotation; 

    private void Awake()
    {
        // 记录初始状态（用于旋转计算和复位）
        if (rotatePivot != null)
        {
            initialOffset = (Vector2)transform.position - (Vector2)rotatePivot.position;
        }
        initialRotation = transform.eulerAngles.z;
    }
    public void RotateAroundPivot(float angle)
    {
        if (rotatePivot == null)
        {
            Debug.LogError("PushedObject: 请设置旋转支点（rotatePivot）！");
            return;
        }
        Vector2 rotatedOffset = Quaternion.Euler(0, 0, angle) * initialOffset;
        transform.position = (Vector2)rotatePivot.position + rotatedOffset;
        transform.eulerAngles = new Vector3(0, 0, initialRotation + angle);
    }
    public void ResetState()
    {
        if (rotatePivot != null)
        {
            transform.position = (Vector2)rotatePivot.position + initialOffset;
        }
        transform.eulerAngles = new Vector3(0, 0, initialRotation);
    }
}