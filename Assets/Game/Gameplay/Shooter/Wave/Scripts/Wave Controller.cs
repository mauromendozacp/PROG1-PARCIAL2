using System.Collections;

using UnityEngine;

public class WaveController : MonoBehaviour
{
    [SerializeField] private float spawnDelay = 0f;
    [SerializeField] private float waveTime = 0f;
    [SerializeField] private Transform[] enemySpawns = null;

    private EnemyPoolController enemyPoolController = null;

    private float timer = 0f;

    private void Update()
    {
        UpdateTimer();
    }

    public void Init(EnemyPoolController enemyPoolController, Transform enemyMainTarget)
    {
        this.enemyPoolController = enemyPoolController;
        this.enemyPoolController.Init();
        this.enemyPoolController.SetMainTarget(enemyMainTarget);

        StartWave();
    }

    public void StartWave()
    {
        timer = 0f;

        StartCoroutine(SpawnEnemiesCoroutine());
        IEnumerator SpawnEnemiesCoroutine()
        {
            while (timer < waveTime)
            {
                yield return new WaitForSeconds(spawnDelay);

                enemyPoolController.SpawnEnemy(GetRandomSpawn());
            }
        }
    }

    public void StopWave()
    {
        StopAllCoroutines();
    }

    private Vector3 GetRandomSpawn()
    {
        if (enemySpawns.Length == 0) return transform.position;

        int index = Random.Range(0, enemySpawns.Length);
        return enemySpawns[index].position;
    }

    private void UpdateTimer()
    {
        timer += Time.deltaTime;
    }
}
