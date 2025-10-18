using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    
    [HideInInspector] public int entranceIndex;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("People"))
        {
            PortalManager.Instance.HandleTeleport(this.transform, other.transform);
        }
    }
}
