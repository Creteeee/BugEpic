using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class BugController : Singleton<BugController>
{
    [Header("虫子状态")]
    public BugStates bugStates = BugStates.Move;
    public GameObject bugSprite;
    [Header("路径")]
    public GameManager.UIType uiType = GameManager.UIType.NonDialogue;
    public Material footPrintMat;
    public GameObject wayPointsGroup;
    public Transform[] waypoints;
    public float Speed = 3f;
    private Tween pathTween;
    private Vector3[] path;
 
   
    void Start()
    {
        //查找wayPointsGroup
        //返回wayPointsFroup下的所有子物体
        InitializeWayPointsGroup();
        waypoints = wayPointsGroup.GetComponentsInChildren<Transform>()
            .Skip(1)
            .ToArray();
       
    }

 
    void Update()
    {
        if (bugStates == BugStates.Move)
        {
            BugMove();
        }
        
    }
    /// <summary>
    /// 虫子是活动的还是静止的
    /// </summary>
    public enum BugStates
    {
        Move,Still,Talk
    }


    public void ChangeState(BugStates state)
    {
        bugStates = state;
    }

    void BugMove()
    {
        path = new Vector3[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
            path[i] = waypoints[i].position;

        switch (uiType)
        {
            case GameManager.UIType.NonDialogue:
                GameManager.Instance.playerState = GameManager.PlayerState.Active;
                pathTween = transform.DOPath(path, (1/Speed) * waypoints.Length, PathType.CatmullRom, PathMode.Ignore)
                    .SetEase(Ease.Linear)
                    .OnUpdate(UpdateOrientation)
                    .OnComplete(SlideUI);
                break;
            case GameManager.UIType.Dialogue:
                GameManager.Instance.playerState = GameManager.PlayerState.Dialogue;
                
                
                break;
            case GameManager.UIType.Beginning:
                GameManager.Instance.playerState = GameManager.PlayerState.Active;
                pathTween = transform.DOPath(path, (1/Speed) * waypoints.Length, PathType.CatmullRom, PathMode.Ignore)
                    .SetEase(Ease.Linear)
                    .OnUpdate(UpdateOrientation)
                    .OnComplete(SHowPV);
                break;
                
        }

        bugStates = BugStates.Still;
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("触发器与物体发生碰撞: " + other.gameObject.name);
        footPrintMat.SetFloat("_isFootPrint",1);
        Debug.Log("撞上");
    }
    

    //每次换界面也要初始化
    public void InitializeWayPointsGroup()
    {
        GameObject wgp;
        switch (uiType)
        {
            case  GameManager.UIType.NonDialogue:
                wgp = Resources.Load("Prefab/WayPointsGroup/WayPointsGroup_NonDialogue") as GameObject;
                if (wayPointsGroup!=null)
                {
                    Destroy(wayPointsGroup.gameObject);
                    wayPointsGroup = null;
                }
                wayPointsGroup = Instantiate(wgp, GameObject.Find("UI").transform);
                break;
            case GameManager.UIType.Dialogue:
                wgp = Resources.Load("Prefab/WayPointsGroup/WayPointsGroup_Dialogue") as GameObject;
                if (wayPointsGroup!=null)
                {
                    Destroy(wayPointsGroup.gameObject);
                    wayPointsGroup = null;
                }
                wayPointsGroup = Instantiate(wgp, GameObject.Find("UI").transform);
                break;
        }
    }

    #region UI
    
    private void SHowPV()
    {
        UIManaer_HomePage_1.Instance.ShowBenginPV();
    }

    private void SlideUI()
    {
        UIManager_Whole.Instance.MoveAwayWholeUI(UIManager_Whole.Instance.gameWindow_Content);
        UIManager_Whole.Instance.MoveAwayWholeUI(this.gameObject);
        UIManager_Whole.Instance.activeUIIndex += 1;
    }


    #endregion
}
