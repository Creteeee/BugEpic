using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager_Whole : Singleton<UIManager_Whole>
{
    //GameWindow
    [NonSerialized]public GameObject gameWindow;
    [NonSerialized]public GameObject gameWindow_Content;
    [NonSerialized]public float horizenOffset;
    public float backOffset = 20;
    
    //Dialogue
    [SerializeField]private GameObject Speaker;//还没找
    private GameObject bubbleGroup;
    private GameObject bubblePrefab;
    [SerializeField]private List<GameObject> bubbles;
    private int dialogueIndex = 0;
    private DialogueData  dialogueData;
    
    //ChangeUIPage
    public int activeUIIndex = 0;
    [SerializeField] private GameObject[] UIPages;
    [SerializeField] private string UIState;

    
    void Start()
    {
        gameWindow = GameObject.Find("UI/GameWindow/Mask").transform.gameObject;
        gameWindow_Content = gameWindow.transform.Find("Content").gameObject;
        StartCoroutine(InitAfterFrame());
        bubbleGroup =  GameObject.Find("UI/GameWindow/Mask/Speaker/BubbleGroup");
        bubblePrefab = Resources.Load("Prefab/Bubbles/Bubble_1") as GameObject;
        Scene scene = SceneManager.GetActiveScene();
        dialogueData = Resources.Load<DialogueData>("Data/"+scene.name);
        RefreshPlayerStateByUITag();
        Speaker = GameObject.Find("UI/GameWindow/Mask/Speaker");
        activeUIIndex = 0;

    }

    void Update()
    {
        switch (GameManager.Instance.playerState)
        {
            case GameManager.PlayerState.Active:
                break;
            case GameManager.PlayerState.Dialogue:
                if (dialogueData.lines.Count>0)
                {
                    if (dialogueIndex >= dialogueData.lines.Count)
                    {
                        GameManager.Instance.playerState =  GameManager.PlayerState.Active;
                        foreach (var bubble in bubbles)
                        {
                            Destroy(bubble);
                        }
                        bubbles.Clear();
                        dialogueIndex = 0;
                        Speaker.GetComponent<DOTweenAnimation>().DORewind();
                        break;
                    }
                    else if (Input.GetMouseButtonDown(0))
                    {
                        dialogueIndex += 1;
                        if (dialogueIndex != dialogueData.lines.Count)
                        {
                            UpdateDialogue();   
                        }
                    }
                }
                if (bubbles.Count == 0)
                {
                    UpdateDialogue();
                    dialogueIndex += 1;
                }
                break;
        }
    }
    IEnumerator InitAfterFrame()
    {
        yield return null; // 确保Canvas刷新完
        RectTransform rt = gameWindow.GetComponent<RectTransform>();
        Debug.Log($"实际宽度: {rt.rect.width}"); // 这时应该是 1350
        horizenOffset =rt.rect.width;
    }
    
    public void MoveAwayWholeUI(GameObject gameObject)
    {
      
        Sequence mySequence = DOTween.Sequence();
        RectTransform rt = gameObject.GetComponent<RectTransform>();
        mySequence.Append(rt.DOAnchorPos(rt.anchoredPosition + new Vector2(backOffset, 0), 0.3f));
        mySequence.Append(rt.DOAnchorPos(rt.anchoredPosition + new Vector2(-(UIManager_Whole.Instance.horizenOffset), 0), 
            1.5f)).OnComplete(RefreshPlayerStateByUITag);
        

    }
    public void InvokeBugMove()
    {
        BugController.Instance.bugStates = BugController.BugStates.Move;
    }
    public void StopBugMove()
    {
        BugController.Instance.bugStates = BugController.BugStates.Still;
    }

    #region Dialogue

    void UpdateDialogue()
    {
        GameObject bubble = Instantiate(bubblePrefab, bubbleGroup.transform);
        TMP_Text text = bubble.transform.Find("Text").GetComponent<TMP_Text>();
        if (text != null)
        {
            Debug.Log("有文本");
        }
        if (dialogueData.lines[dialogueIndex] != null)
        {
            Debug.Log("有对话");
        }
        text.text = dialogueData.lines[dialogueIndex].englishText;
        
        bubbles.Add(bubble);

        if (bubbles.Count > 3)
        {
            GameManager.Instance.playerState = GameManager.PlayerState.Froze;
            GameObject oldBubble = bubbles[0];

            oldBubble.transform.DOScale(0, 0.3f).OnComplete(() =>
            {
                ActivePlayerState();
                RemoveOldBubble();
                
            });
            Vector2 pos = bubbleGroup.GetComponent<RectTransform>().anchoredPosition;
            MoveBubbleGroup(bubbleGroup.GetComponent<RectTransform>(),pos,bubblePrefab.GetComponent<RectTransform>());
        }
    }
    
    public async void MoveBubbleGroup(RectTransform bubbleGroup, Vector2 pos, RectTransform bubblePrefab)
    {
        await bubbleGroup.DOAnchorPos(
            pos + new Vector2(0, bubblePrefab.rect.height),
            0.3f
        ).AsyncWaitForCompletion();

        Debug.Log("动画结束！");
        bubbleGroup.anchoredPosition = pos;
    }

    void RemoveOldBubble()
    {
        if (bubbles.Count > 0)
        {
            var first = bubbles[0];
            bubbles.RemoveAt(0);
            Destroy(first);
        }
    }
    #endregion
    
    #region ChangePlayerState

    void ActivePlayerState()
    {
        GameManager.Instance.playerState = GameManager.PlayerState.Dialogue;
    }

    /// <summary>
    /// 一次性的UI状态刷新
    /// </summary>
    void RefreshPlayerStateByUITag()
    {
        UIState = UIPages[activeUIIndex].tag;
        switch (UIState)
        {
            case "NonDialogue":
                GameManager.Instance.playerState = GameManager.PlayerState.Active;
                //Speaker.SetActive(false);
                break;
            case "Dialogue":
                GameManager.Instance.playerState = GameManager.PlayerState.Dialogue;
                Speaker.GetComponent<DOTweenAnimation>().DOPlay();
                //Speaker.SetActive(true);
                break;
        }
    }
    #endregion
}
