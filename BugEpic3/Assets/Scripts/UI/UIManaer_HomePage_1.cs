using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManaer_HomePage_1 : Singleton<UIManaer_HomePage_1>
{
    public GameObject PV;
    public int Time = 4;
    public string SceneTo;

    public void InvokeBugMove()
    {
        BugController.Instance.bugStates = BugController.BugStates.Move;
    }

    public void showSuggest(GameObject obj)
    {
        obj.SetActive(true);
    }
    public void hideSuggest(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void LoadGameWindow(GameObject gameObject)
    {
        gameObject.GetComponent<CanvasGroup>().DOFade(1, 0.2f);
        gameObject.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f);
    }

    public void ShowBenginPV( )
    {
        PV.GetComponent<CanvasGroup>().DOFade(1, 0.2f);
        StartCoroutine(LoadSceneAfterDelay(SceneTo, Time));
    }


    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
      
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
    
}
