using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 velocity;
    public float speed;
    public float rotation;
    public float lifetime = 3f, startTime = 60f;
    public int index;
    public Transform target;
    private List<Collider2D> enemies = new List<Collider2D>();
    public DamageType element;
    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        Destroy(gameObject, lifetime);
        switch (index)
        {
            case 1:
                target = character.shuriken.transform;
                break;
            case 2:
                target = character.shuriken.transform;
                break;
        }
    }
    void FixedUpdate()
    {
        
        switch (index)
        {
            case 0:
                transform.Translate(velocity * speed * Time.fixedDeltaTime);
                break;
            case 1:
                transform.position = Vector3.MoveTowards(transform.position,target.transform.position,speed/100f);
                break;
            case 2:
                transform.position = target.transform.position;
                break;
        }
    }

    public CharacterObject character;
    public int projectileIndex, attackIndex = 0;
    public string tagToHit = "Enemy";
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(tagToHit) && other.gameObject != character.gameObject && !enemies.Contains(other))
        {
            
            IHittable victim = other.transform.root.GetComponent<IHittable>();
            if (victim != null && projectileIndex > 0)
            {
                enemies.Add(other);
                victim.Hit(character, projectileIndex, attackIndex, element);
                Destroy(gameObject, .1f);
            }
        }
    }
}
