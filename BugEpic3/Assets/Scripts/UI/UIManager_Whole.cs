using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager_Whole : Singleton<UIManager_Whole>
{
    //GameWindow
    [NonSerialized]public GameObject gameWindow;
    [NonSerialized]public GameObject gameWindow_Content;
    [NonSerialized]public float horizenOffset;
    public float backOffset = 20;
    public GameObject B_HomePage;
    public GameObject B_Chapters;
    public GameObject B_TellStory;
    
    //Dialogue
    [SerializeField]private GameObject Speaker;//还没找
    private GameObject bubbleGroup;
    private GameObject bubblePrefab;
    private TMP_Text captions;
    [SerializeField]private List<GameObject> bubbles;
    private int dialogueIndex = 0;
    private DialogueData  dialogueData;
    
    //ChangeUIPage
    public int activeUIIndex = 0;
    [SerializeField] private GameObject[] UIPages;
    [SerializeField] private string UIState;
    
    //Loading
    public GameObject loadingWindow;


    IEnumerator WaitForGameManager()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
    }
    
    void Start()
    {
        StartCoroutine(WaitForGameManager());
        gameWindow = GameObject.Find("UI/GameWindow/Mask").transform.gameObject;
        gameWindow_Content = gameWindow.transform.Find("Content").gameObject;
        StartCoroutine(InitAfterFrame());
        bubbleGroup =  GameObject.Find("UI/GameWindow/Mask/Content/B_TellStory/Speaker/BubbleGroup");
        bubblePrefab = Resources.Load("Prefab/Bubbles/Bubble_1") as GameObject;
        Scene scene = SceneManager.GetActiveScene();
        dialogueData = Resources.Load<DialogueData>("Data/"+scene.name);
        RefreshPlayerStateByUITag();
        Speaker = GameObject.Find("UI/GameWindow/Mask/Content/B_TellStory/Speaker");
        activeUIIndex = 0;
        loadingWindow =  GameObject.Find("UI/GameWindow/Mask/LoadingWindow"); 
        B_HomePage = GameObject.Find("UI/GameWindow/Mask/Content/B_HomePage");
        B_Chapters = GameObject.Find("UI/GameWindow/Mask/Content/B_Chapters");
        B_TellStory = GameObject.Find("UI/GameWindow/Mask/Content/B_TellStory");
        captions = GameObject.Find("UI/GameWindow/Mask/Content/B_TellStory/Captions").GetComponent<TMP_Text>();
        Speaker.SetActive(false);

    }

    void Update()
    {
        switch (GameManager.Instance.playerState)
        {
            case GameManager.PlayerState.Active:
                break;
            case GameManager.PlayerState.Dialogue:
                Speaker.SetActive(true);
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
                        //Speaker.GetComponent<DOTweenAnimation>().DORewind();
                        BugController.Instance.BugMove((() => {Speaker.SetActive(false);}));
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
        // 先判断是否有已经存在的气泡，若有，先销毁旧的气泡
        if (bubbles.Count > 0)
        {
            Destroy(bubbles[0]);  // 销毁旧气泡
            bubbles.Clear(); // 清空列表
        }

        // 创建新的气泡
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

        text.text = dialogueData.lines[dialogueIndex].customText;
        captions.text = dialogueData.lines[dialogueIndex].chineseText;

        bubbles.Add(bubble); // 将新气泡添加到列表中

        // 当气泡数量超过 1 时，处理气泡动画
        if (bubbles.Count > 1)
        {
            // 停止玩家对话，处理气泡移动和销毁
            GameManager.Instance.playerState = GameManager.PlayerState.Froze;
            GameObject oldBubble = bubbles[0];

            // 动画效果：气泡缩小消失
            oldBubble.transform.DOScale(0, 0.3f).OnComplete(() =>
            {
                ActivePlayerState();  // 恢复玩家状态
                RemoveOldBubble();    // 移除旧气泡
            });

            // 移动气泡组，确保新气泡不覆盖
            Vector2 pos = bubbleGroup.GetComponent<RectTransform>().anchoredPosition;
            MoveBubbleGroup(bubbleGroup.GetComponent<RectTransform>(), pos, bubblePrefab.GetComponent<RectTransform>());
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
            bubbles.RemoveAt(0);  // 移除列表中的旧气泡
            Destroy(first);       // 销毁旧气泡
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

    #region TransitionScene

    public void LoadingNextScene(System.Action onComplete)
    {
        loadingWindow.SetActive(true);
        Slider slider = loadingWindow.transform.Find("Slider").gameObject.GetComponent<Slider>();
        slider.value = 0;
        slider.DOValue(1, 6).SetEase(Ease.Linear).OnComplete(() =>
        {
            FinishLoadingNextScene();
            onComplete?.Invoke(); // 动画完成后，调用回调函数（销毁物体）
        });
    }

    public void FinishLoadingNextScene()
    {
        RectTransform rtContent = GameObject.Find("UI/GameWindow/Mask/Content").GetComponent<RectTransform>();
        rtContent.anchoredPosition = new Vector2(0, 0);
        B_HomePage.SetActive(false);
        B_Chapters.SetActive(false);
        B_TellStory.SetActive(false);
        loadingWindow.SetActive(false);
    }
    

    #endregion
}
