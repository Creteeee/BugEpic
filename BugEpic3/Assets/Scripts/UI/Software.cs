using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Software : MonoBehaviour
{
    public GameObject downBarSoftwarePrefab;
    public GameObject softwareWindow;
    [SerializeField] private GameObject downBarSoftwareInst;
    public Transform DownBarSoftwareRoot;

    void Start()
    {
        
    }
    public void OpenSoftware()
    {
        softwareWindow.SetActive(true);
        if (downBarSoftwareInst == null)
        {
            downBarSoftwareInst = Instantiate(downBarSoftwarePrefab, DownBarSoftwareRoot);   
        }
    }
    public void CancelSoftware()
    {
        if (downBarSoftwareInst!=null)
        {
            Destroy(downBarSoftwareInst.gameObject);
            downBarSoftwareInst = null;
        }
        softwareWindow.SetActive(false);
    }

    public void OverlayOnTop()
    {
        softwareWindow.transform.SetAsLastSibling();
    }
}
