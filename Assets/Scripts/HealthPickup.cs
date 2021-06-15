using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int healAmount;

    AudioManager audio;
    private SpriteRenderer[] renderers;
    private Rigidbody2D rb;

    private bool collected = false;
    private bool falling = false;
    private float upForce = 3, sideForce = 3, fallForce = 50;//force for flinging the collectables
    private float fallMin = .4f, fallmax = 1, startingY, endingY;//how far to let fall before stoppin

    void Start()
    {
        //Get refs
        audio = AudioManager.instance;
        renderers = GetComponentsInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        //make sure it's scaled correctly
        transform.localScale = Vector3.one;//set position and scale correctly
        transform.localPosition = Vector3.zero;
        startingY = transform.position.y;
        endingY = UnityEngine.Random.Range(fallMin, fallmax);

        //physics for collecting
        falling = true;
        FlingCollectable();//make him go!
    }
    private void FixedUpdate()
    {
        if (Mathf.Abs(transform.position.y - startingY) > endingY)
            falling = false;

        if (falling == true)
        {
            rb.AddForce(Vector2.down * fallForce);
        }
        else rb.velocity = Vector2.zero;
    }
    void FlingCollectable()
    {
        //add force up
        rb.AddForce(Vector2.up * upForce, ForceMode2D.Impulse);
        //randomly choose a side to add force to
        int rand = UnityEngine.Random.Range(0, 2);
        if (rand == 0) { rb.AddForce(Vector2.right * sideForce, ForceMode2D.Impulse); }
        else if (rand == 1) { rb.AddForce(Vector2.left * sideForce, ForceMode2D.Impulse); }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && !collected && !falling)//stop the player from collecting twice or while it's in the air
        {
            StartCoroutine(CollectItem(collision));
            collected = true;
        }
    }

    private IEnumerator CollectItem(Collider2D playerColl)
    {
        audio.PlaySound("HealthPickup");
        playerColl.GetComponentInParent<HealthManager>().AddHealth(healAmount);

        //animate and remove
        foreach (SpriteRenderer rend in renderers)
        {
            rend.DOFade(0, 1).SetDelay(1f);
        }
        renderers[1].transform.SetParent(null);
        renderers[0].transform.DOScale(3, 1).SetDelay(1f);
        renderers[0].transform.DOLocalMoveY(1, 1f);
        renderers[0].transform.DOLocalRotate(new Vector3(0f, 0f, 45f), .5f).SetDelay(1f);
        renderers[0].transform.DOLocalRotate(new Vector3(0f, 0f, 90f), .25f).SetDelay(1.5f);
        renderers[0].transform.DOLocalRotate(new Vector3(0f, 0f, 180f), .25f).SetDelay(1.75f);

        yield return new WaitForSeconds(1);
    }
}