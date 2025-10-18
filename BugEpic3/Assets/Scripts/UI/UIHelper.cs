using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIHelper : MonoBehaviour
{
    //GameWindow
    [NonSerialized]public GameObject gameWindow;
    [NonSerialized]public GameObject gameWindow_Content;
    [NonSerialized]public float horizenOffset;
    public float backOffset = 20;

    private void Start()
    {
        gameWindow = GameObject.Find("UI/GameWindow/Mask").transform.gameObject;
        gameWindow_Content = gameWindow.transform.Find("Content").gameObject;
        RectTransform rt = gameWindow.GetComponent<RectTransform>();
        horizenOffset =rt.rect.width/2;
    }

    public void MoveSliceRight(GameObject gameObject)
    {
        Sequence mySequence = DOTween.Sequence();
        RectTransform rt = gameObject.GetComponent<RectTransform>();
        mySequence.Append(rt.DOAnchorPos(rt.anchoredPosition - new Vector2(backOffset, 0), 0.3f));
        mySequence.Append(rt.DOAnchorPos(rt.anchoredPosition - new Vector2(-horizenOffset, 0), 
            1.5f));
    }
    
    public void MoveSliceLeft(GameObject gameObject)
    {
        Sequence mySequence = DOTween.Sequence();
        RectTransform rt = gameObject.GetComponent<RectTransform>();
        mySequence.Append(rt.DOAnchorPos(rt.anchoredPosition + new Vector2(backOffset, 0), 0.3f));
        mySequence.Append(rt.DOAnchorPos(rt.anchoredPosition + new Vector2(-horizenOffset, 0), 
            1.5f));
    }
   
}
