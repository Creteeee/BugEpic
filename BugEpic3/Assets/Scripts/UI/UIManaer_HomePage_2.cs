using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIManaer_HomePage_2 : MonoBehaviour
{
    public void MoveAwayWholeUI(GameObject gameObject)
    {
      
        Sequence mySequence = DOTween.Sequence();
        
        mySequence.Append(gameObject.transform.DOMove(gameObject.transform.position + new Vector3(20, 0, 0), 0.3f));
        mySequence.Append(gameObject.transform.DOMove(gameObject.transform.position + new Vector3(-150, 0, 0), 1.5f));
    }
}
