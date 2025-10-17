using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public PlayerState playerState = PlayerState.Active;
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
    
    
}
