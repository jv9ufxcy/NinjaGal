using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SophiesHealthManager : MonoBehaviour
{


public enum UIType { player, enemy }


    [SerializeField] public float maxHealth = 100, minHealth = 100;
    [SerializeField] public int intValue = 0;
    [SerializeField] public Image HealthFill, DamageFill, BarImage;

    [SerializeField] public float showHealthTime = 1, fadeOutTime = .5f, damageShowTime = 1, damageShowSpeed = 1f;
    [SerializeField] public Color HealthColor, DamageColor, BackColor;
    public bool IsDead = false;
    private Color invisible = new Color(0, 0, 0, 0);
    [HideInInspector, SerializeField] public UIType ui = UIType.player;
    [SerializeField] public GameObject HealthPickupPrefab;

    private float currentHealth = 100, damageShowTimer, healthBarFadeTimer;
    private bool isHealing = false, coroutineStarted = false, healthIsVisible = false, playerIsInvincible, dizzyEnemy = false, deathCoroutineStarted = false;
    private CharacterObject character;
    private CharacterController controller;

    private void Start()
    {
        character = GetComponentInParent<CharacterObject>();
        controller = GetComponent<CharacterController>();
        ResetHealthToMax();
        UpdateFill();

        if (ui == UIType.enemy)
        {
            HideHealth();
        }
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
        HealthFill.fillAmount = currentHealth / 100;
    }

    private void UpdateFillForHeal()
    {
        DamageFill.fillAmount = currentHealth / 100f;
    }

    private void Update()
    {
        //DECREASE TIMERS
        damageShowTimer -= Time.deltaTime;
        healthBarFadeTimer -= Time.deltaTime;

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


        if (healthBarFadeTimer < 0)//if we need to start fading health out
        {
            if (!coroutineStarted && healthIsVisible)
                StartCoroutine(FadeHealth());
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

        if (ui == UIType.enemy)
        {
            if (healthBarFadeTimer > 0)//if you get hit while it's fading
            {
                ShowHealth();//go back to showing
            }

            if (healthBarFadeTimer <= 0)//as long as that timer isn't running, we've done a successful fade and can set the bool to false
                healthIsVisible = false;
        }

        coroutineStarted = false;
    }


    void ResetHealthToMax()
    {
        currentHealth = maxHealth;
    }

    public void AddHealth(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;


        damageShowTimer = damageShowTime;//set the timer back to max when injured happens

        isHealing = true;
        UpdateFillForHeal();
    }

    public void RemoveHealth(int amount)
    {
        damageShowTimer = damageShowTime;//set the timer back to max when injured happens
        healthBarFadeTimer = showHealthTime;//reset timer for showing health bar here too

        //reset attack combo

        currentHealth -= amount;

        if (currentHealth <= minHealth)
        {
            currentHealth = minHealth;
            if (!deathCoroutineStarted)
                StartCoroutine(DeathEvent(false));
        }

        if (ui == UIType.enemy)
        {
            //dizzy check
            if (currentHealth <= (maxHealth * .3) && !dizzyEnemy)//if 30% or less health and we're not dizzy
            {
                //DoDizzy();
            }

            ShowHealth();//show the health bar
        }

        UpdateFill();

    }
    //public void DoDizzy()
    //{
    //    //stop
    //    GetComponent<NavMeshAgent>().acceleration = 80;
    //    GetComponent<NavMeshAgent>().isStopped = true;//stop immediately

    //    //dizzy
    //    character.StartStateFromScript(5);//start dizzy state
    //    GetComponentInChildren<SkinnedMeshRenderer>().material = DizzyMat;

    //    //AUDIO dizzy sound
    //    character.aniHealth = 1;//dizzy anims

    //    dizzyEnemy = true;//be dizzy
    //    //remove material
    //    //play particle

    //}

    private void ShowHealth()
    {
        BarImage.color = BackColor;
        HealthFill.color = HealthColor;
        DamageFill.color = DamageColor;

        healthIsVisible = true;
    }

    //public void FinisherDeath()
    //{
    //    if (!deathCoroutineStarted)
    //        StartCoroutine(DeathEvent(true));
    //    GameEngine.gameEngine.Screenshake();
    //}

    /// <summary>
    /// waits until the death animation is done and then destroys the character
    /// </summary>
    /// <returns></returns>
    IEnumerator DeathEvent(bool spawnHealth)
    {
        deathCoroutineStarted = true;
        IsDead = true;

        if (character.controlType == CharacterObject.ControlType.AI)//if not player
            controller.detectCollisions = false;//please no collisions after die

        if (spawnHealth)
            Instantiate(HealthPickupPrefab, transform.position, transform.rotation);

        yield return new WaitForSeconds(3);//get length of death animation        

        if (character.controlType == CharacterObject.ControlType.PLAYER)
        {
            //RESPAWN HERE
        }

        Destroy(this.gameObject);
    }
}
