using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Pool;

public class EnemyPoolController : MonoBehaviour
{
    [SerializeField] private EnemyController enemyPrefab = null;
    [SerializeField] private Transform enemyHolder = null;

    private ObjectPool<EnemyController> enemyPool = null;
    private List<EnemyController> enemyList = null;
    private Transform enemyMainTarget = null;

    public List<EnemyController> EnemyList => enemyList;

    public void Init()
    {
        enemyPool = new ObjectPool<EnemyController>(CreateEnemy, GetEnemy, ReleaseEnemy, DestroyEnemy);
        enemyList = new List<EnemyController>();
    }

    public void SpawnEnemy(Vector3 spawnPosition)
    {
        EnemyController enemy = enemyPool.Get();
        enemy.transform.position = spawnPosition;
    }

    public void SetMainTarget(Transform enemyMainTarget)
    {
        this.enemyMainTarget = enemyMainTarget;

        enemyList.ForEach((enemy) => enemy.SetMainTarget(enemyMainTarget));
    }

    private EnemyController CreateEnemy()
    {
        EnemyController enemy = Instantiate(enemyPrefab, enemyHolder);
        enemy.Init((e) => enemyPool.Release(e));

        return enemy;
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