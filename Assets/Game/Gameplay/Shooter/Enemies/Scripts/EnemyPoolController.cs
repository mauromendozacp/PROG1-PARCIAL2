using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Pool;

[Serializable]
public class EnemyData
{
    public string id = string.Empty;
    public EnemyController prefab = null;
}

public class EnemyPoolController : MonoBehaviour
{
    [SerializeField] private EnemyData[] enemies = null;

    private Dictionary<string, ObjectPool<EnemyController>> enemyPoolDict = null;
    private List<EnemyController> enemyList = null;
    private Transform enemyMainTarget = null;

    public List<EnemyController> EnemyList => enemyList;

    public void Init(Camera mainCamera)
    {
        enemyPoolDict = new Dictionary<string, ObjectPool<EnemyController>>();
        enemyList = new List<EnemyController>();

        for (int i = 0; i < enemies.Length; i++)
        {
            EnemyController enemyPrefab = enemies[i].prefab;
            string enemyId = enemies[i].id;

            GameObject enemyHolder = new GameObject(enemyId + "_holder");
            enemyHolder.transform.SetParent(transform);

            enemyPoolDict.Add(enemyId, new ObjectPool<EnemyController>(
                createFunc: () =>
                {
                    EnemyController enemy = Instantiate(enemyPrefab, enemyHolder.transform);
                    enemy.Init(mainCamera, (e) => enemyPoolDict[enemyId].Release(e));

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

    private EnemyController GetRandomEnemy()
    {
        int randomIndex = UnityEngine.Random.Range(0, enemies.Length);
        string enemyId = enemies[randomIndex].id;

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