using System.Collections.Generic;
using UnityEngine;

public class DestroyWhileTouchTag : MonoBehaviour
{
    [SerializeField] private List<string> tags;
    [SerializeField] private bool isDestroy = true;
    void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (tags.Contains(collision.gameObject.tag))
        {
            if (isDestroy)
                Destroy(gameObject);
            else
                collision.gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
      
        if (tags.Contains(collision.gameObject.tag))
        {
            if (isDestroy)
                Destroy(gameObject);
            else collision.gameObject.SetActive(false);
        }
    }
}
