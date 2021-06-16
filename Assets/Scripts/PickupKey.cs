using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupKey : MonoBehaviour
{
    [SerializeField] private int healAmount;

    AudioManager audio;
    private bool collected = false;

    void Start()
    {
        //Get refs
        audio = AudioManager.instance;
    }

    private void OnTriggerStay2D(Collider2D coll)
    {
        if (!collected && coll.CompareTag("Player"))
        {
            audio.PlaySound("Gem");
            coll.GetComponentInParent<CharacterObject>().keyCount++;
            collected = true;
            Destroy(gameObject, .1f);
        }

    }
}
