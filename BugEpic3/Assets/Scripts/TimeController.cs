using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;

public class TimeController : MonoBehaviour
{
     [Header("UI")]
    public TMP_Text timeText;
    public TMP_Text timeText2;// 显示时间的文本
    public Button leftButton;
    public Button rightButton;
    public RectTransform Celestral; // 注意改成 RectTransform

    [Header("长按设置")]
    public float longPressDelay = 0.5f;   // 判定长按的时间
    public float repeatInterval = 0.2f;   // 长按持续增加的间隔

    [Header("天体移动")]
    public float movePer20Min = 22.22f;   // 每 20 分钟平移的宽度

    private DateTime currentTime;

    void Start()
    {
        // 从文本解析初始时间
        if (!DateTime.TryParseExact(timeText.text, "HH:mm", null, System.Globalization.DateTimeStyles.None, out currentTime))
        {
            Debug.LogWarning("文本时间格式不正确，默认使用当前时间");
            currentTime = DateTime.Now;
        }

        UpdateTimeText();

        // 左按钮减 20 分钟，右按钮加 20 分钟
        AddButtonEvents(leftButton, Subtract20Minutes);
        AddButtonEvents(rightButton, Add20Minutes);
    }

    void UpdateTimeText()
    {
        timeText.text = currentTime.ToString("HH:mm");
        timeText2.text = currentTime.ToString("HH:mm"); 
    }

    void Add20Minutes()
    {
        currentTime = currentTime.AddMinutes(20);
        UpdateTimeText();

        if (Celestral != null)
        {
            Celestral.anchoredPosition += new Vector2(movePer20Min, 0);
        }
    }

    void Subtract20Minutes()
    {
        currentTime = currentTime.AddMinutes(-20);
        UpdateTimeText();

        if (Celestral != null)
        {
            Celestral.anchoredPosition += new Vector2(-movePer20Min, 0);
        }
    }

    #region 按钮长按处理
    void AddButtonEvents(Button btn, Action onClick)
    {
        EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = btn.gameObject.AddComponent<EventTrigger>();

        // PointerDown
        EventTrigger.Entry entryDown = new EventTrigger.Entry();
        entryDown.eventID = EventTriggerType.PointerDown;
        entryDown.callback.AddListener((data) => { StartCoroutine(LongPressRoutine(onClick)); });
        trigger.triggers.Add(entryDown);

        // PointerUp
        EventTrigger.Entry entryUp = new EventTrigger.Entry();
        entryUp.eventID = EventTriggerType.PointerUp;
        entryUp.callback.AddListener((data) => { StopAllCoroutines(); });
        trigger.triggers.Add(entryUp);
    }

    System.Collections.IEnumerator LongPressRoutine(Action onClick)
    {
        // 短按立即触发一次
        onClick.Invoke();

        // 等待长按判定
        float timer = 0f;
        while (timer < longPressDelay)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // 长按持续触发
        while (true)
        {
            onClick.Invoke();
            yield return new WaitForSeconds(repeatInterval);
        }
    }
    #endregion
    public void OpenTimeTab(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }
    
    public void CloseTimeTab(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
