using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Transform Captions;
    public TMP_Text speakerWords;
    public GameObject timeLineHolder;
    public GameObject windowsNotePad;
    public GameObject downBarNotePad;
    public GameObject NotePadWindow;
    public GameObject cancel;
    public Vector2 initialAnchorPos;
    private int timer0 = 0;
    private int timer1 = 0;
    private double savedTime = 0;

    private void Start()
    {
        windowsNotePad.GetComponent<DraggableApp>().enabled = false;
        downBarNotePad.GetComponent<Button>().interactable = false;
        cancel.GetComponent<Button>().interactable = false;
        downBarNotePad.GetComponent<Button>().onClick.AddListener(TestTutorial2);
        cancel.GetComponent<Button>().onClick.AddListener(TestTutorial3);
    }

    private void Update()
    {
        if ( Captions.childCount>0)
        {
            speakerWords = Captions.GetComponent<TMP_Text>();
            if (speakerWords.text == "您一定能够理解我在说什么吧!")
            {
                if (timer0 < 1)
                {
                    timeLineHolder.GetComponent<PlayableDirector>().Play();
                    initialAnchorPos = NotePadWindow.GetComponent<RectTransform>().anchoredPosition;
                    timer0 += 1;
                    GameManager.Instance.playerState = GameManager.PlayerState.Active;
                }
                TestTutorial1();
                
            }
        }

        
    }

    public void StopTimeline()
    {
        // timeLineHolder.GetComponent<PlayableDirector>().time = timeLineHolder.GetComponent<PlayableDirector>().time; // 锁定当前时间
        // timeLineHolder.GetComponent<PlayableDirector>().Evaluate();           // 刷新状态
        // timeLineHolder.GetComponent<PlayableDirector>().Pause();              // 暂停播放
        //
        windowsNotePad.GetComponent<DraggableApp>().enabled = true;
        savedTime = timeLineHolder.GetComponent<PlayableDirector>().time;
        timeLineHolder.GetComponent<PlayableDirector>().Pause();
        Debug.Log(savedTime);
    }

    public void EndTimeline()
    {
        timeLineHolder.SetActive(false);
        GameManager.Instance.playerState = GameManager.PlayerState.Dialogue;
    }

    public void TestTutorial1()
    {
        Vector2 rtPos = NotePadWindow.GetComponent<RectTransform>().anchoredPosition;
        if (Vector2.Distance(rtPos,initialAnchorPos) >= 300 && timer1 < 1)
        {
            timeLineHolder.GetComponent<PlayableDirector>().time = savedTime;
            timeLineHolder.GetComponent<PlayableDirector>().Play();
            downBarNotePad.GetComponent<Button>().interactable = true;
            timer1 += 1;
        }
    }

    public void TestTutorial2()
    {
        cancel.GetComponent<Button>().interactable = true;
        downBarNotePad.GetComponent<Button>().onClick.RemoveListener(TestTutorial2);
        timeLineHolder.GetComponent<PlayableDirector>().time = savedTime;
        timeLineHolder.GetComponent<PlayableDirector>().Play();
        

        
    }
    
    public void TestTutorial3()
    {
        timeLineHolder.GetComponent<PlayableDirector>().time = savedTime;
        timeLineHolder.GetComponent<PlayableDirector>().Play();
        windowsNotePad.GetComponent<Button>().onClick.RemoveListener(TestTutorial3);
    }
}
