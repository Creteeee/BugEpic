using DG.Tweening;
using UnityEngine;

public class RayReflect2D : MonoBehaviour
{
    [Header("射线参数")]
    public Transform firePoint;          // 发射点
    public Transform targetPoint;        // 可选目标点
    public LayerMask hitLayers;          // 检测层
    public Vector2 customDirection = Vector2.right; // 自定义方向
    public bool useCustomDirection = false;
    public int maxReflections = 3;
    public float maxDistance = 100f;
    public Material sunMat;
    public GameObject Ajimide;
    private int timer = 0;
    public GameObject bubble;
    public Transform bubblePlace;

    [Header("LineRenderer参数")]
    public float startWidth = 0.02f;
    public float endWidth = 0.1f;
    public Gradient colorGradient;

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();

        line.startWidth = startWidth;
        line.endWidth = endWidth;
        line.colorGradient = colorGradient;
        line.positionCount = 0;

        line.material = new Material(Shader.Find("Unlit/Color"));
        line.material.color = Color.yellow;
    }

    void Update()
    {
        DrawReflectedLine();
    }

    void DrawReflectedLine()
    {
        if (firePoint == null) return;

        Vector2 dir = useCustomDirection
            ? customDirection.normalized
            : (targetPoint ? ((Vector2)targetPoint.position - (Vector2)firePoint.position).normalized : Vector2.right);

        Vector2 origin = firePoint.position;

        // 用来存储每个反射点
        Vector3[] positions = new Vector3[maxReflections + 2];
        int posIndex = 0;

        positions[posIndex++] = origin;

        for (int i = 0; i <= maxReflections; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, maxDistance, hitLayers);

            if (hit.collider != null)
            {
                Vector3 pos = new Vector3(hit.point.x, hit.point.y, 0);
                positions[posIndex++] = pos;

                // 命中目标检测
                if (targetPoint != null && hit.collider.transform == targetPoint)
                {
                    line.startColor = Color.red;
                    line.endColor = Color.red;
                    if (timer<1)
                    {
                        timer += 1;
                        Ajimide.GetComponent<RectTransform>().DOAnchorPos(Ajimide.GetComponent<RectTransform>().anchoredPosition + new Vector2(400, 0), 1.5f).OnComplete(BugMove);
                        Instantiate(bubble, bubblePlace);
                    }
                    Debug.Log("打中目标！");
                    break;
                }

                // 计算反射方向
                dir = Vector2.Reflect(dir, hit.normal);

                // 微小偏移避免再次检测同一个点
                origin = hit.point + dir * 0.01f;
            }
            else
            {
                positions[posIndex++] = origin + dir * maxDistance;
                break;
            }
        }

        // 更新 LineRenderer
        line.positionCount = posIndex;
        for (int i = 0; i < posIndex; i++)
        {
            line.SetPosition(i, positions[i]);
        }
    }

    // 编辑器可视化 Gizmo
    void OnDrawGizmos()
    {
        if (firePoint == null) return;

        Gizmos.color = Color.cyan;
        Vector2 dir = useCustomDirection
            ? customDirection.normalized
            : (targetPoint ? ((Vector2)targetPoint.position - (Vector2)firePoint.position).normalized : Vector2.right);

        Gizmos.DrawLine(firePoint.position, firePoint.position + (Vector3)dir * 200);
        Gizmos.DrawSphere(firePoint.position, 0.05f);
    }

    void BugMove()
    {
        BugController.Instance.BugMove( ()=>{});
    }
}
