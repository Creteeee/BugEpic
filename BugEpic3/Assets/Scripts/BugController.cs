using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class BugController : MonoBehaviour
{
    public BugStates bugStates = BugStates.Move;
    public GameObject bugSprite;
    public GameObject wayPointsGroup;
    public Transform[] waypoints;
    public float duration = 3f;
    private Tween pathTween;
    private Vector3[] path;
   
    void Start()
    {
        //查找wayPointsGroup
        //返回wayPointsFroup下的所有子物体
        waypoints = wayPointsGroup.GetComponentsInChildren<Transform>()
            .Skip(1)
            .ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        if (bugStates == BugStates.Move)
        {
            BugMove();
        }
        
    }
    
    public enum BugStates
    {
        Move,Still,Talk
    }

    void BugMove()
    {
        path = new Vector3[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
            path[i] = waypoints[i].position;

        // 启动 DOTween 路径动画
        pathTween = transform.DOPath(path, duration, PathType.CatmullRom, PathMode.Ignore)
            .SetEase(Ease.Linear)
            .OnUpdate(UpdateOrientation);
        bugStates = BugStates.Still;
    }

    void ChangeState(BugStates state)
    {
        bugStates = state;
    }
    void UpdateOrientation()
    {
        float t = pathTween.ElapsedPercentage();
        Vector3 next = pathTween.PathGetPoint(Mathf.Min(1f, t + 0.001f));
        Vector2 dir = (next - transform.position).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // 让 Sprite 的 Y 轴对齐切线方向
        bugSprite.transform.rotation = Quaternion.Euler(0, 0, angle - 90f); 
    }
    
}
