using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public int minEnemies = 4, maxEnemies=12;
    public GameObject[] enemies;
    public Transform[] spawnPoints;
    public enum State { idle, battle, concluded}
    public State state;
    private List<Transform> spawnList = new List<Transform>();
    private List<CharacterObject> enemiesSpawned = new List<CharacterObject>();
    // Start is called before the first frame update
    void Start()
    {
        spawnList.AddRange(spawnPoints);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (enemiesSpawned.Count<=0&&state==State.battle)
        {
            state = State.concluded;
        }
    }
    private void LateUpdate()
    {
        if (state == State.battle)
        {
            enemiesSpawned.RemoveAll(e => e.controlType == CharacterObject.ControlType.DEAD);
        }
    }
    public void SpawnEnemies()
    {
        int enemyNum = Random.Range(minEnemies, maxEnemies);
        for (int i = 0; i < enemyNum; i++)
        {
            int e = Random.Range(0, enemies.Length);
            GameObject clone = Instantiate(enemies[e], spawnList[i].position, Quaternion.identity) as GameObject;
            enemiesSpawned.Add(clone.transform.GetComponent<CharacterObject>());
        }
    }
    private void RandomizeList()
    {
        for (int i = 0; i < spawnList.Count; i++)
        {
            Transform temp = spawnList[i];
            int randomIndex = Random.Range(i, spawnList.Count);
            spawnList[i] = spawnList[randomIndex];
            spawnList[randomIndex] = temp;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (state == State.idle)
            {
                SpawnEnemies();
                RandomizeList();
                state = State.battle;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (state == State.concluded)
            {
                state = State.idle;
            }
        }
    }
}
