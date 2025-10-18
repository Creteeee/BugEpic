using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
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
       
        if (index == -1) 
        {
         
            return;
        }
    
        if (Time.time < entranceCooldowns[entrance])
        {
       
            return;
        }


        StartCoroutine(SwitchPos(entrance, targetObj,index));
        entranceCooldowns[entrance] = Time.time + cooldown;
    }

    private IEnumerator SwitchPos(Transform entrance, Transform targetObj,int index)
    {
        GameObject TargetObj = targetObj.gameObject;
        Image TargetImg = TargetObj.GetComponent<Image>();
        TargetImg.enabled = false;
        yield return new WaitForSeconds(0.5f);
        targetObj.position = targetPositions[index].position;
        TargetImg.enabled = true;
    }
    private int GetEntranceIndex(Transform entrance)
    {
     
        string startNames = "";
        foreach (var pos in startPositions)
        {
            startNames += pos != null ? pos.name + "，" : "空引用，";
        }
      
    
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
