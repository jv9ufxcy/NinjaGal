using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    private bool opened = false;
    AudioManager audiomanager;
    void Start()
    {
        audiomanager = AudioManager.instance;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player") && !opened)
        {
            CharacterObject player = collision.transform.GetComponent<CharacterObject>();
            if (player.keyCount > 0 )
            {
                Door();
            }
            else
                audiomanager.PlaySound("Locked");
        }
        
    }

    void Door()
    {
        opened = true;
        audiomanager.PlaySound("ChestOpen");
        gameObject.SetActive(false);
    }
}
