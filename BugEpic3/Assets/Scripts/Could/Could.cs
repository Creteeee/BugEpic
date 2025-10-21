
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Could : MonoBehaviour
{

    [SerializeField] private GameObject[] rain;
    [SerializeField] private ParticleSystem RainSystem;
    [SerializeField] private GameObject water;
    public int Maxwater;
    public RectTransform position;
    private BoxCollider2D box;
    private bool isRaining;
    [SerializeField] private RectTransform parent;
    private IEnumerator currentRainCoroutine;
    // Start is called before the first frame update
      void Start()
    {
        box = GetComponent<BoxCollider2D>();
    }

   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cup"))
        {
            isRaining = true;
            Debug.Log("杯子进入，开始下雨");
            if (currentRainCoroutine != null)
            {
                StopCoroutine(currentRainCoroutine);
            }
            currentRainCoroutine = Raining();
            StartCoroutine(currentRainCoroutine);
            HideRainObjects();
        }
    }

    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Cup"))
        {
            isRaining = false;
            Debug.Log("杯子离开，停止下雨");

           StopRaining();
           HideRainObjects();
        }
    }

    public void StopRaining()
    {
        if (currentRainCoroutine != null)
        {
            StopCoroutine(currentRainCoroutine);
            currentRainCoroutine = null; 
        }

        ShowRainObjects();
    }
    private IEnumerator Raining()
    {
        // 循环生成雨滴，直到达到最大数量或被停止
        for (int i = 0; i < Maxwater; i++)
        {
            // 在指定区域内随机生成雨滴（基于rainArea的位置）
            float randomX = Random.Range(position.position.x - 10, position.position.x + 10);
            Vector3 dropPos = new Vector3(randomX, position.position.y, position.position.z);
            // 实例化雨滴到父物体下
            Instantiate(water, dropPos, Quaternion.identity, parent);
            // 每0.1秒生成一个，保持均匀间隔
            yield return new WaitForSeconds(0.1f);
        }
    }

    // 隐藏其他雨效果
    private void HideRainObjects()
    {
        foreach (var obj in rain)
        {
            if (obj != null && isRaining)
            {
                obj.GameObject().SetActive(false);
            }

            if (obj != null && !isRaining)
            {
                obj.GameObject().SetActive(true);
            }
        }
    }

    // 显示其他雨效果
    private void ShowRainObjects()
    {
        foreach (var obj in rain)
        {
            if (obj != null)
                obj.GameObject().SetActive(true);
        }
    }

    // 清除所有已生成的雨滴
    private void ClearRainDrops()
    {
        // 遍历父物体下的所有雨滴子对象并销毁
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
    
}
