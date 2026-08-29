using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private ShelfRegistry shelfRegistry;

    [Header("Wandering")]
    [SerializeField] private Transform[] wanderPoints;

    [Header("Enemies")]
    [SerializeField]
    private List<EnemySpawnData> enemies =
        new List<EnemySpawnData>();

    private void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        foreach (EnemySpawnData enemyData in enemies)
        {
            if (enemyData.spawnPoint == null)
            {
                Debug.LogWarning(
                    "Enemy Spawn Point is missing"
                );

                continue;
            }

            GameObject enemyObject =
                Instantiate(
                    enemyPrefab,
                    enemyData.spawnPoint.position,
                    enemyData.spawnPoint.rotation
                );

            // =========================
            // 1. Give Wander Points
            // =========================

            EnemyWander wander =
                enemyObject.GetComponent<EnemyWander>();

            if (wander != null)
            {
                wander.SetWanderPoints(wanderPoints);

                Debug.Log(
                    "Wander Points assigned: "
                    + wanderPoints.Length
                );
            }

            // =========================
            // 2. Start Shopping
            // =========================

            EnemyShopping shopping =
                enemyObject.GetComponent<EnemyShopping>();

            if (shopping != null)
            {
                shopping.Initialize(
                    enemyData.shoppingList,
                    shelfRegistry
                );
            }
        }
    }
}