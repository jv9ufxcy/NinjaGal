using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderTrigger : MonoBehaviour
{
    public UnityEvent OnPlayerEnterTrigger;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Player player = collision.GetComponent<Player>();
        if (collision.CompareTag("Player"))
        {
            OnPlayerEnterTrigger.Invoke();
        }
    }
}
