using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public PlayerState playerState = PlayerState.Active;
    public List<GameObject> levels;
    public static int levelIndex = 0;
    public enum PlayerState
    {
        Dialogue,Froze,Active
    }
    /// <summary>
    /// 这一页UI荷马是否会说话
    /// </summary>
    public enum UIType
    {
        NonDialogue,Dialogue,Beginning
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void FinishLevel() // 游戏章节外放置加载动画，章节内不用
    {
        Transform parentTransform = GameObject.Find("UI/GameWindow/Mask/Content").transform;
        GameObject previousLevel = parentTransform.GetChild(0).gameObject;
        GameObject newLevel = Instantiate(levels[levelIndex].gameObject, parentTransform);
        newLevel.transform.SetSiblingIndex(0);

        // 让 BugMove 完成后，加载下一个场景
        BugController.Instance.BugMove(() =>
        {
            // 在加载新场景时完成动画后销毁物体
            UIManager_Whole.Instance.LoadingNextScene(() =>
            {
                Destroy(previousLevel.gameObject);  // 在加载完成后销毁之前的关卡
            });
        });

        levelIndex += 1;
    }
    


    
}
