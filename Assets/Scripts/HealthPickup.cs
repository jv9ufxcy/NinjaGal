using DG.Tweening;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int healAmount, kiAmount;
    public bool tracking = false;
    public float timeToTrack = 1;
    AudioManager audioManager;
    private SpriteRenderer[] renderers;
    private bool collected = false;
    public Vector2 velocity;
    public float speed;
    public float rotation;
    public string pickupSound = "HealthPickup";
    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        //Get refs
        audioManager = AudioManager.instance;
        transform.SetParent(null);
    }
    private void FixedUpdate()
    {
        if (tracking)
        {
            if (timeToTrack>0)
            {
                transform.Translate(velocity * speed * Time.fixedDeltaTime);
                timeToTrack -= Time.fixedDeltaTime;
            }
            else
                transform.position = Vector3.MoveTowards(transform.position,GameEngine.gameEngine.mainCharacter.transform.position,1);
        }
    }
    private void OnTriggerStay2D(Collider2D coll)
    {
        if (!collected&&coll.CompareTag("Player")&&timeToTrack<=0)
        {
            audioManager.PlaySound(pickupSound);
            coll.GetComponentInParent<HealthManager>().AddHealth(healAmount);
            coll.GetComponentInParent<CharacterObject>().ChangeMeter(kiAmount);
            collected = true;
            Destroy(gameObject, .1f);
        }

    }
}