using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public CharacterObject player, target;
    private enum State { Equipped,Thrown,Recalling,Hooked}
    private State state;
    private Animator anim;
    private Rigidbody2D rb;
    private TrailRenderer tr;
    private SpriteRenderer sr;
    public float recallSpeed = 15f, grabDist=2f;
    public float timerMax = 120f, timer=0f;
    private AudioManager audioManager;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        sr = GetComponent<SpriteRenderer>();
        state = State.Recalling;
    }
    private void Start()
    {
        audioManager = AudioManager.instance;
    }
    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Equipped:
                break;
            case State.Thrown:
                //TryGrabShuriken();
                timer--;
                if (timer<=0f)
                {
                    Recall();
                }
                break;
            case State.Recalling:
                Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
                rb.velocity = dirToPlayer * recallSpeed;
                TryGrabShuriken();
                break;
            default:
                break;
        }
    }

    private void TryGrabShuriken()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < grabDist)
        {
            tr.enabled = false;
            state = State.Equipped;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            audioManager.PlaySound("ButtonClick");
            player.QuickChangeForm(0);
        }
    }

    private void LateUpdate()
    {
        switch (state)
        {
            case State.Equipped:
                anim.SetFloat("shurikenState", 0);
                transform.position = player.transform.position;
                if (player.FacingDir.y == 1)
                    sr.sortingOrder = player.spriteRend.sortingOrder + 10;
                else
                    sr.sortingOrder = player.spriteRend.sortingOrder - 10;
                break;
            case State.Thrown:
                anim.SetFloat("shurikenState", 1);
                break;
            case State.Recalling:
                anim.SetFloat("shurikenState", 1);
                break;
            case State.Hooked:

                anim.SetFloat("shurikenState", -1);
                if (target != null)
                    transform.position = target.transform.position;
                else
                    Recall();
                
                break;
            default:
                break;
        }
    }
    public void ThrowShuriken(Vector3 dir)
    {
        player.QuickChangeForm(player.formIndex);
        timer = timerMax;
        rb.isKinematic = false;
        transform.position = player.transform.position+dir.normalized;
        rb.AddForce(dir, ForceMode2D.Impulse);
        tr.enabled = true;
        audioManager.PlaySound("Shuriken");
        state = State.Thrown;
    }
    public void Recall()
    {
        state = State.Recalling;
    }
    public bool IsEquipped()
    {
        return state == State.Equipped;
    }
    public int atkIndex = 7;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (state == State.Thrown)
        {
            IHittable victim = collision.transform.root.GetComponent<IHittable>();
            if (victim != player.GetComponent<IHittable>() && victim != null)
            {
                state = State.Hooked;
                CinemachineShake.instance.ShakeCamera(1, 0.2f);
                target = collision.transform.root.GetComponent<CharacterObject>();
                victim.Hit(player,7, 0, null);
            }
        }
    }
}
