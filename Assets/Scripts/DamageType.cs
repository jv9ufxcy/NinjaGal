using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ElemenalDamage/DamageType Type")]
public class DamageType : ScriptableObject
{
    public GameObject damageEffect;
    public string soundEffect = "Flame";

    public void SpawnPrefabEffect(Vector3 spawnPosition)
    {
        GameObject clone = Instantiate(damageEffect, spawnPosition, Quaternion.identity) as GameObject;
        clone.transform.SetParent(null);
        AudioManager.instance.PlaySound(soundEffect);
    }
}
