using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour, IHittable
{
    public GameObject drop, effect;
    public string soundEffect = "RockBreak1";
    private AudioManager audioManager;
    public int chance=6;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.instance;
    }
    public void Hit(CharacterObject attacker, int projectileIndex, int atkIndex, DamageType element)
    {
        Break();
    }

    private void Break()
    {
        int rand = Random.Range(1, 10);
        if (drop != null&&rand>chance)
        {
            GameObject go = Instantiate(drop, transform.position, Quaternion.identity);
        }
        if (effect != null)
        {
            GameObject dust = Instantiate(effect, transform.position, Quaternion.identity);
        }
        audioManager.PlaySound(soundEffect);
        Destroy(gameObject, .1f);
    }
}
