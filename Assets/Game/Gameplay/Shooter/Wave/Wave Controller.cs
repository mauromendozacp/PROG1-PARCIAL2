using System.Collections;

using UnityEngine;

public class WaveController : MonoBehaviour
{
    [SerializeField] private EnemyPoolController enemyPoolController = null;
    [SerializeField] private float spawnDelay = 0f;
    [SerializeField] private float waveTime = 0f;
    [SerializeField] private Transform enemyMainTarget = null;
    [SerializeField] private Transform[] enemySpawns = null;

    private float timer = 0f;

    private void Start()
    {
        enemyPoolController.Init();
        enemyPoolController.SetMainTarget(enemyMainTarget);

        StartWave();
    }

    private void Update()
    {
        UpdateTimer();
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

    private Vector3 GetRandomSpawn()
    {
        if (enemySpawns.Length == 0) return Vector3.zero;

        int index = Random.Range(0, enemySpawns.Length);
        return enemySpawns[index].position;
    }

    private void UpdateTimer()
    {
        timer += Time.deltaTime;
    }
}
