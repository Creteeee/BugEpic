using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftwareDownBar : MonoBehaviour
{
   public GameObject softwareWindow;
   public SoftwareType type = SoftwareType.Other;

   public void OverlayOnTop()
   {
      int lastIndex = softwareWindow.transform.parent.childCount - 1;
      int targetIndex = Mathf.Max(0, lastIndex - 3); 
      softwareWindow.transform.SetSiblingIndex(targetIndex);
      if (type == SoftwareType.Other)
      {
         GameManager.Instance.playerState = GameManager.PlayerState.Froze;
         Time.timeScale = 0;
      }
      else
      {
         GameManager.Instance.playerState = GameManager.PlayerState.Dialogue;
         Time.timeScale = 1;
      }
   }

   public enum SoftwareType
   {
      Game,Other
   }
}
