using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Pool;

public class EnemyPoolController : MonoBehaviour
{
    [SerializeField] private EnemyData[] enemies = null;

    private Dictionary<ENEMY_TYPE, ObjectPool<EnemyController>> enemyPoolDict = null;
    private List<EnemyController> enemyList = null;
    private Transform enemyMainTarget = null;

    private ENEMY_TYPE[] availableEnemies = null;

    public List<EnemyController> EnemyList => enemyList;

    public void Init(Camera mainCamera, Action onKillEnemy)
    {
        enemyPoolDict = new Dictionary<ENEMY_TYPE, ObjectPool<EnemyController>>();
        enemyList = new List<EnemyController>();

        for (int i = 0; i < enemies.Length; i++)
        {
            EnemyController enemyPrefab = enemies[i].prefab;
            ENEMY_TYPE enemyId = enemies[i].id;

            GameObject enemyHolder = new GameObject(enemyId.ToString().ToLower() + "_holder");
            enemyHolder.transform.SetParent(transform);

            enemyPoolDict.Add(enemyId, new ObjectPool<EnemyController>(
                createFunc: () =>
                {
                    EnemyController enemy = Instantiate(enemyPrefab, enemyHolder.transform);
                    enemy.Init(mainCamera, onKillEnemy, (e) => enemyPoolDict[enemyId].Release(e));

                    return enemy;
                }, 
                GetEnemy, ReleaseEnemy, DestroyEnemy));
        }
    }

    public void SpawnEnemy(Vector3 spawnPosition)
    {
        EnemyController enemy = GetRandomEnemy();
        enemy.transform.position = spawnPosition;
        enemy.ToggleAgent(true);
    }

    public void SetMainTarget(Transform enemyMainTarget)
    {
        this.enemyMainTarget = enemyMainTarget;

        enemyList.ForEach((enemy) => enemy.SetMainTarget(enemyMainTarget));
    }

    public void SetAvailableEnemies(ENEMY_TYPE[] availableEnemies)
    {
        this.availableEnemies = availableEnemies;
    }

    private EnemyController GetRandomEnemy()
    {
        if (availableEnemies == null || availableEnemies.Length == 0) return null;

        List<EnemyData> enemyList = enemies.Where(e => availableEnemies.Contains(e.id)).ToList();
        int randomIndex = UnityEngine.Random.Range(0, enemyList.Count);
        ENEMY_TYPE enemyId = enemyList[randomIndex].id;

        return enemyPoolDict[enemyId].Get();
    }

    private void GetEnemy(EnemyController enemy)
    {
        enemy.SetMainTarget(enemyMainTarget);
        enemy.OnGet();
        enemyList.Add(enemy);
    }

    private void ReleaseEnemy(EnemyController enemy)
    {
        enemy.OnRelease();
        enemyList.Remove(enemy);
    }

    private void DestroyEnemy(EnemyController enemy)
    {
        Destroy(enemy.gameObject);
    }
}