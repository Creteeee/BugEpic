using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CupMouthDetector : MonoBehaviour
{
    public Could pourController;
    [Header("检测参数")]
    public int minStayBalls = 2; // 至少2个向上运动的小球停在杯口
    public float stayThreshold = 0.2f; // 静止0.3秒视为堆积
    public float upSpeedThreshold = 0.1f; // y轴速度>0.1视为“向上运动”

    // 跟踪杯口区域内“向上进入”的小球及其静止时间
    private Dictionary<GameObject, float> upwardBalls = new Dictionary<GameObject, float>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            GameObject ball = other.gameObject;
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
            if (rb == null) return;
            if (rb.velocity.y > upSpeedThreshold)
            {
               
                if (!upwardBalls.ContainsKey(ball))
                {
                    upwardBalls.Add(ball, 0);
                    Debug.Log("检测到下方堆积的小球进入杯口");
                    pourController.StopRaining();
                }
            }
         
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            GameObject ball = other.gameObject;
            if (upwardBalls.ContainsKey(ball))
            {
                upwardBalls.Remove(ball); 
            }
        }
    }

    private void Update()
    {
       
        // 1. 清理已销毁的小球（先创建副本遍历，避免修改原集合时出错）
        List<GameObject> destroyedBalls = new List<GameObject>();
        // 遍历副本：upwardBalls.Keys.ToList()
        foreach (var ball in upwardBalls.Keys.ToList())
        {
            if (ball == null)
                destroyedBalls.Add(ball);
        }
        // 统一从原集合中移除
        foreach (var ball in destroyedBalls)
            upwardBalls.Remove(ball);

        // 2. 更新跟踪小球的静止时间（同样遍历副本）
        // 遍历副本：upwardBalls.ToList()
        foreach (var ball in upwardBalls.ToList())
        {
            GameObject ballObj = ball.Key;
            Rigidbody2D rb = ballObj.GetComponent<Rigidbody2D>();
            if (rb == null) continue;

            // 静止判定：速度很小（几乎不动）
            if (rb.velocity.sqrMagnitude < 0.01f)
            {
                upwardBalls[ballObj] += Time.deltaTime; // 累加静止时间
            }
            else
            {
                upwardBalls[ballObj] = 0; // 还在动，重置静止时间
            }
        }

        // 3. 统计满足“静止时间达标”的小球数量（遍历副本）
        int qualifiedBalls = 0;
        foreach (var ball in upwardBalls.ToList())
        {
            if (ball.Value >= stayThreshold)
                qualifiedBalls++;
        }

        
        if (qualifiedBalls >= minStayBalls && pourController != null)
        {
            Debug.Log("达到阈值，暂停倒球");
            pourController.StopRaining();
        }
    }
}