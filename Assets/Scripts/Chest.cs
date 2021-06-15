using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject healthPickup;
    private Animator anim;
    private bool opened = false;
    AudioManager audiomanager;
    void Start()
    {
        anim = GetComponent<Animator>();
        audiomanager = AudioManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")&& !opened)
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        opened = true;
        anim.Play("ChestOpening");
        anim.SetFloat("Blend", 1f);
        audiomanager.PlaySound("ChestOpen");
        GameObject healthInstance = Instantiate(healthPickup, this.gameObject.transform);
    }
}
