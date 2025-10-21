using UnityEngine;
using UnityEngine.UI; // 引入UI命名空间

public class PushedObject : MonoBehaviour
{
    [Header("旋转设置")]
    public RectTransform rotatePivot; // 改为RectTransform（UI支点）
    [HideInInspector] public Vector2 initialOffset; // 基于UI本地坐标的初始偏移
    [HideInInspector] public float initialRotation; // 初始Z轴旋转角度

    private RectTransform rectTransform; // 自身的RectTransform

    private void Awake()
    {
        // 获取自身RectTransform组件
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("PushedObject必须挂载RectTransform组件！");
            return;
        }

        if (rotatePivot == null)
        {
            Debug.LogError("请先设置旋转支点（rotatePivot，需为RectTransform）！");
            return;
        }

        // 初始化：计算UI本地坐标系下的初始偏移（关键修改）
        initialOffset = rectTransform.anchoredPosition - rotatePivot.anchoredPosition;
        // 记录初始Z轴旋转角度
        initialRotation = rectTransform.eulerAngles.z;
    }

    /// <summary>
    /// 绕支点旋转指定角度（UI坐标系）
    /// </summary>
    public void RotateAroundPivot(float angle)
    {
        if (rotatePivot == null || rectTransform == null) return;

        // 核心：将初始偏移绕Z轴旋转angle角度（UI坐标系内计算）
        Vector2 rotatedOffset = Quaternion.Euler(0, 0, angle) * initialOffset;
        // 新位置 = 支点的UI位置 + 旋转后的偏移（确保绕支点旋转）
        rectTransform.anchoredPosition = rotatePivot.anchoredPosition + rotatedOffset;
        // 同步更新自身旋转
        rectTransform.eulerAngles = new Vector3(0, 0, initialRotation + angle);
    }

    /// <summary>
    /// 重置到初始状态（UI坐标系）
    /// </summary>
    public void ResetState()
    {
        if (rotatePivot == null || rectTransform == null) return;

        // 重置UI位置
        rectTransform.anchoredPosition = rotatePivot.anchoredPosition + initialOffset;
        // 重置旋转
        rectTransform.eulerAngles = new Vector3(0, 0, initialRotation);
    }
}