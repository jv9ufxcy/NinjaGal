using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthSystem
{
    public const int MAX_FRAGMENTS = 2;
    public event EventHandler OnDamaged, OnHealed, OnDead;
    private List<Heart> heartList;
    public HealthSystem(int healthAmount)
    {
        heartList = new List<Heart>();
        for (int i = 0; i < healthAmount; i++)
        {
            Heart heart = new Heart(2);
            heartList.Add(heart);
        }

        //heartList[heartList.Count - 1].SetFragments(0);
    }
    public List<Heart> GetHeartList()
    {
        return heartList;
    }
    public void Damage(int damageAmt)
    {
        for(int i=heartList.Count-1;i>=0;i--)//cycle through all hearts
        {
            Heart heart = heartList[i];
            if (damageAmt>heart.GetFragmentAmount())//test heart health versus damage
            {
                damageAmt -= heart.GetFragmentAmount();
                heart.Damage(heart.GetFragmentAmount());
            }
            else
            {
                heart.Damage(damageAmt);
                break;
            }
        }
        if (OnDamaged!=null)
        {
            OnDamaged(this, EventArgs.Empty);
        }
        if (IsDead())
        {
            if (OnDead != null)
            {
                OnDead(this, EventArgs.Empty);
            }
        }
    }
    public void Heal(int healAmt)
    {
        for(int i=0;i < heartList.Count;i++)//cycle through all hearts
        {
            Heart heart = heartList[i];
            int missingFragments = MAX_FRAGMENTS - heart.GetFragmentAmount();
            if (healAmt>missingFragments)
            {
                healAmt -= missingFragments;
                heart.Heal(missingFragments);
            }
            else
            {
                heart.Heal(healAmt);
                    break;
            }
        }
        if (OnHealed != null)
        {
            OnHealed(this, EventArgs.Empty);
        }
    }
    public bool IsDead()
    {
        return heartList[0].GetFragmentAmount() == 0;
    }
    public class Heart
    {
        private int fragments;
        public Heart(int fragments)
        {
            this.fragments = fragments;
        }
        public int GetFragmentAmount()
        {
            return fragments;
        }
        public void SetFragments(int fragments)
        {
            this.fragments = fragments;
        }
        public void Damage(int damageAmt)
        {
            if (damageAmt>=fragments)
            {
                fragments = 0;
            }
            else
            {
                fragments -= damageAmt;
            }
        }
        public void Heal(int healAmt)
        {
            if (fragments+healAmt>MAX_FRAGMENTS)
            {
                fragments = MAX_FRAGMENTS;
            }
            else
            {
                fragments += healAmt;
            }
        }
    }
}
public class HealthManager : MonoBehaviour
{
    public enum UIType { AI, PLAYER, BOSS }
    public UIType UI = UIType.PLAYER;
    public float maxHealth = 100, currentShieldHealth = 0, currentHealth, desperationHealth;
    public int maxPoise = 10, currentPoise = 10;
    public Image HealthFill, DamageFill, BarImage, MeterFill;

    public float showHealthTime = 1, fadeOutTime = .5f, damageShowTime = 1, damageShowSpeed = 1f;
    public bool IsDead = true, shouldSpawnHealth = true, isDesperation;
    public Color HealthColor, DamageColor, BackColor;
    private Color invisible = new Color(0, 0, 0, 0);
    public int numOfPickups = 3;
    public GameObject keyPickup;

    public Animator effectsAnim, charAnim;
    private float currentMeter = 100, damageShowTimer, healthBarFadeTimer;
    private bool isHealing = false, coroutineStarted = false, healthIsVisible = false, deathCoroutineStarted = false;
    private CharacterObject character;
    public UnityEvent OnDeath;
    public Material dizzyMat;
    public SpriteRenderer rend, effectsRend;
    private AudioManager audioManager;

    [HideInInspector] public Respawn respawn;
    [SerializeField] private Transform damagePopup;
    //private PlayerRespawner respawner;
    public bool HasShield()
    {
        return currentShieldHealth > 0;
    }
    private void Start()
    {
        audioManager = AudioManager.instance;

        if (audioManager == null)
        {
            Debug.LogError("No Audio Manager in Scene");
        }
        character = GetComponent<CharacterObject>();

        SetMaxHealth();
        switch (UI)
        {
            case UIType.AI:
                
                break;
            case UIType.PLAYER:
                ShowHealth();
                respawn = GetComponent<Respawn>();
                break;
            case UIType.BOSS:
                HideHealth();
                break;
            default:
                break;
        }
    }

    public void SetMaxHealth()
    {
        //    PoiseReset();
        currentHealth = maxHealth;
        //    if(rend!=null)
        //        rend.color = Color.white;
    }

    private void HideHealth()
    {
        BarImage.color = invisible;
        DamageFill.color = invisible;
        HealthFill.color = invisible;

        healthIsVisible = false;

    }

    private void UpdateFill()
    {
        HealthFill.fillAmount = currentHealth / maxHealth;
    }

    private void UpdateFillForHeal()
    {
        DamageFill.fillAmount = currentHealth / maxHealth;
    }
    private void Update()
    {
        //DECREASE TIMERS
        damageShowTimer -= Time.deltaTime;
        healthBarFadeTimer -= Time.deltaTime;
        switch (UI)
        {
            case UIType.AI:
                break;
            case UIType.PLAYER:
                UpdateDamage();
                break;
            case UIType.BOSS:
                UpdateDamage();
                break;
            default:
                break;
        }
        

        //if (healthBarFadeTimer < 0)//if we need to start fading health out
        //{
        //    if (!coroutineStarted && healthIsVisible)
        //        StartCoroutine(FadeHealth());
        //}
    }

    private void UpdateDamage()
    {
        if (damageShowTimer < 0)//if the timer is up
        {
            if (HealthFill.fillAmount < DamageFill.fillAmount && isHealing)//if the bars aren't equal and it's healing
            {
                HealthFill.fillAmount += damageShowSpeed * Time.deltaTime;//increase the health bar
            }
            else if (HealthFill.fillAmount < DamageFill.fillAmount)//if the health amount is smaller than the damage show
            {
                DamageFill.fillAmount -= damageShowSpeed * Time.deltaTime;//decrease the damage bar 
            }
            else if (isHealing)//otherwise if the bars are even we're done showing healing so turn the bool off
                isHealing = false;
        }
    }

    private IEnumerator FadeHealth()
    {
        coroutineStarted = true;

        for (float f = fadeOutTime; f > 0; f -= Time.deltaTime)//iterate over time
        {
            Color h = HealthFill.color;//get color
            Color d = DamageFill.color;//gt color
            Color b = BarImage.color;

            h.a = f;//set the alpha to the variable being counted down for
            d.a = f;
            b.a = f;

            HealthFill.color = h;//set that to be our new color
            DamageFill.color = d;
            BarImage.color = b;

            yield return new WaitForEndOfFrame();
        }

        if (healthBarFadeTimer > 0)//if you get hit while it's fading
        {
            ShowHealth();//go back to showing
        }

        if (healthBarFadeTimer <= 0)//as long as that timer isn't running, we've done a successful fade and can set the bool to false
            healthIsVisible = false;

        coroutineStarted = false;
    }
    public void AddHealth(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        damageShowTimer = damageShowTime;//set the timer back to max when injured happens

        isHealing = true;
        switch (UI)
        {
            case UIType.AI:
                break;
            case UIType.PLAYER:
                UpdateFillForHeal();
                break;
            case UIType.BOSS:
                UpdateFillForHeal();
                break;
            default:
                break;
        }
    }

    public void RemoveHealth(int amount, bool isCrit)
    {
        damageShowTimer = damageShowTime;//set the timer back to max when injured happens
        healthBarFadeTimer = showHealthTime;//reset timer for showing health bar here too
        currentHealth -= amount;
        DamagePopup.Create(transform.position, amount, isCrit);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            if (!IsDead)
                Die();
        }
        switch (UI)
        {
            case UIType.AI:
                break;
            case UIType.PLAYER:
                UpdateFill();
                HitCounter.instance.OnPlayerDamaged();
                break;
            case UIType.BOSS:
                UpdateFill();
                break;
            default:
                break;
        }
    }
    //public void PoiseDamage(int amount)
    //{
    //    currentPoise -= amount;
    //}
    //public void PoiseReset()
    //{
    //    currentPoise = maxPoise;
    //}
    public void ShowHealth()
    {
        BarImage.color = BackColor;
        HealthFill.color = HealthColor;
        DamageFill.color = DamageColor;

        healthIsVisible = true;
    }
    public void ChangeMeter(float curMeter)
    {
        currentMeter = curMeter;
        MeterFill.fillAmount = currentMeter / 100f;
    }
    public void Die()
    {
        switch (UI)
        {
            case UIType.AI:
                {
                    audioManager.PlaySound("EnemyDeath");
                    TurnInvisible();
                    TurnNotCollidable();
                    SpawnPickup();
                    character.OnDeath();
                    HitCounter.instance.OnEnemyKilled();
                    Destroy(this.gameObject, 1);
                }
                CinemachineShake.instance.ShakeCamera(1f, .2f);
                character.controlType = CharacterObject.ControlType.DEAD;

                break;
            case UIType.PLAYER://DEATH
                //PLAY DEATH SOUND HERE
                character.controlType = CharacterObject.ControlType.DEAD;//no more move
                CinemachineShake.instance.ShakeCamera(2f, .1f);
                //can move false
                //playdeath sound
                character.OnDeath();
                respawn.RespawnPlayer();
                break;
        }
    }

    private void TurnNotCollidable()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false ;
        }
    }

    void TurnInvisible()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].color = invisible;
        }
    }
    public float minRotation;
    public float maxRotation=360;
    public int numberOfBullets;
    public bool isRandom=false;

    public float cooldown;
    float timer;
    public float bulletSpeed;
    public Vector2 bulletVelocity;


    float[] rotations;
    public float[] RandomRotations()
    {
        for (int i = 0; i < numberOfBullets; i++)
        {
            rotations[i] = UnityEngine.Random.Range(minRotation, maxRotation);
        }
        return rotations;

    }
    public float[] DistributedRotations()
    {
        for (int i = 0; i < numberOfBullets; i++)
        {
            var fraction = (float)i / ((float)numberOfBullets - 1);
            var difference = maxRotation - minRotation;
            var fractionOfDifference = fraction * difference;
            rotations[i] = fractionOfDifference + minRotation; // We add minRotation to undo Difference
        }
        foreach (var r in rotations) print(r);
        return rotations;
    }
    public GameObject[] SpawnBullets()
    {
        if (isRandom)
        {
            // This is in Update because we want a random rotation for each bullet each time
            RandomRotations();
        }

        // Spawn Bullets
        GameObject[] spawnedBullets = new GameObject[numberOfBullets];
        for (int i = 0; i < numberOfBullets; i++)
        {
            spawnedBullets[i] = Instantiate(keyPickup, transform);

            var b = spawnedBullets[i].GetComponent<HealthPickup>();
            b.rotation = rotations[i];
            b.speed = bulletSpeed;
            b.velocity = bulletVelocity;
        }
        return spawnedBullets;
    }
    private void SpawnPickup()
    {
        numberOfBullets = HitCounter.instance.hitCount + 2;
        rotations = new float[numberOfBullets];
        DistributedRotations();
        SpawnBullets();
        //for (int i = 0; i < HitCounter.instance.hitCount+2; i++)
        //{
        //    int randNumX = UnityEngine.Random.Range(-20, 20);
        //    int randNumY = UnityEngine.Random.Range(15, 35);
        //    Vector2 offsetDir = new Vector2(randNumX, randNumY);
        //    GameObject effect = Instantiate(pickup, transform.position, transform.rotation);
        //    effect.GetComponentInChildren<Rigidbody2D>().AddForce(offsetDir, ForceMode2D.Impulse);
        //    effect.transform.SetParent(null);
        //}
    }

}