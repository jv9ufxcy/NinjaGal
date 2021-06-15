using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int healAmount;

    AudioManager audio;
    private SpriteRenderer[] renderers;
    private bool collected = false;

    void Start()
    {
        //Get refs
        audio = AudioManager.instance;
    }
  
    private void OnTriggerStay2D(Collider2D coll)
    {
        if (!collected&&coll.CompareTag("Player"))
        {
            audio.PlaySound("HealthPickup");
            coll.GetComponentInParent<HealthManager>().AddHealth(healAmount);
            collected = true;
            Destroy(gameObject, .1f);
        }

    }
}