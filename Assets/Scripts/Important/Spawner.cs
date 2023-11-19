using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    float SpawnCooldown = 10f;
    bool CanSpawn;
    public GameObject EnemyPrefab;
    public Vector3 offset;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SpawnCooldown -= Time.deltaTime;

        if (SpawnCooldown <= 0)
        {
            CanSpawn = true; 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))// && CanSpawn == true)
        {
            GameObject enemy = Instantiate(EnemyPrefab, transform.position + offset, transform.rotation);
            Debug.Log(enemy);
            SpawnCooldown = 10f;
            CanSpawn = false;
            Debug.Log("spawnigtrigger");
        }
    }
}
