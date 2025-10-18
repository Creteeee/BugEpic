using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : Portal
{
    public static PortalManager Instance;
    [SerializeField] private Transform[] targetPositions;
    [SerializeField] private Transform[] startPositions;
    [SerializeField] private float cooldown = 0.5f;
    private Dictionary<Transform, float> entranceCooldowns = new Dictionary<Transform, float>();

    // Start is called before the first frame update
    private void Awake()
    {
        // 单例初始化（确保唯一）
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        InitializeEntrances();
    }
     private void InitializeEntrances()
    {
       
        if (startPositions.Length != targetPositions.Length)
        {
            Debug.LogError("入口与出口数量不匹配！", this);
            return;
        }
        
        foreach (var entrance in startPositions)
        {
            if (entrance.GetComponent<Collider2D>() == null)
            {
                var collider = entrance.gameObject.AddComponent<BoxCollider2D>();
                collider.isTrigger = true; 
            }
  
            entranceCooldowns[entrance] = 0;
        }
    }


    public void HandleTeleport(Transform entrance, Transform targetObj)
    {
        int index = GetEntranceIndex(entrance);
        if (index == -1) return;
        
        if (Time.time < entranceCooldowns[entrance])
        {
            return;
        }
        targetObj.position = targetPositions[index].position;
        entranceCooldowns[entrance] = Time.time + cooldown;
    }

    private int GetEntranceIndex(Transform entrance)
    {
        for (int i = 0; i < startPositions.Length; i++)
        {
            if (startPositions[i] == entrance)
            {
                return i; 
            }
        }
        return -1;
    }
}
