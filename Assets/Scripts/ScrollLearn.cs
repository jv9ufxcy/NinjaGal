using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollLearn : MonoBehaviour
{
    GameEngine gEngine;
    public Sprite[] scrollSprites;
    private SpriteRenderer rend;
    private bool collected = false;
    private AudioManager audioManager;
    private void Start()
    {
        gEngine = GameEngine.gameEngine;
        audioManager = AudioManager.instance;
        rend = GetComponent<SpriteRenderer>();
        GetNextScroll();
    }

    private void GetNextScroll()
    {
        int nextScrollIndex = gEngine.mainCharacter.maxIndex + 1;
        nextScrollIndex = Mathf.Clamp(nextScrollIndex, 1, 3);
        rend.sprite = scrollSprites[nextScrollIndex];
    }

    private void OnTriggerStay2D(Collider2D coll)
    {
        if (!collected && coll.CompareTag("Player"))
        {
            gEngine.mainCharacter.LearnScroll();
            gEngine.mainCharacter.GlobalPrefab(gEngine.mainCharacter.maxIndex);
            audioManager.PlaySound("Learned");
            Destroy(gameObject, .1f);
            collected = true;
        }
    }
}
