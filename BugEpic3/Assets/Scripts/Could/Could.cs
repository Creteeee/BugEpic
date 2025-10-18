using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Could : MonoBehaviour
{

    [SerializeField] private Object[] rain;
    [SerializeField] private ParticleSystem RainSystem;
    [SerializeField] private GameObject water;
    public int Maxwater;
    public Vector2 position;
    private BoxCollider2D box;
    
    // Start is called before the first frame update
    void Start()
    {
        box = GetComponent<BoxCollider2D>();
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cup"))
        {
            Debug.Log("进来了");
            StartCoroutine(Raining()); 
            //RainSystem.gameObject.SetActive(true);
            for (int i =0;i<rain.Length;i++)
            {
                rain[i].GameObject().SetActive(false);
            }
             
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Cup"))
        {
            Debug.Log("出去了");
            //RainSystem.gameObject.SetActive(false);
            for (int i =0;i<rain.Length;i++)
            {
                rain[i].GameObject().SetActive(true);
            }
        }
    }

    private IEnumerator Raining()
    {
        for (int i=0;i<Maxwater;i++)
        {
            Vector2 pos = new Vector2(Random.Range(position.x-40,position.x+40),position.y);
            Instantiate(water, transform.position, Quaternion.identity, transform);
            yield return new WaitForSeconds(0.05f);
        }
       
    }
  
    
}
