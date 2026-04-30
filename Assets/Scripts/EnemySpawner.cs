//4/30/2026
//Christian Andrion
//Script to periodically spawn enemies

using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnLocation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine("SpawnEnemy");
    }

    //Spawn enemy every 10 seconds
    IEnumerator SpawnEnemy()
    {
        while (true) { 
            GameObject enemy = Instantiate(enemyPrefab,spawnLocation.position,Quaternion.identity);
            yield return new WaitForSeconds(10f);
        }
    }
}
