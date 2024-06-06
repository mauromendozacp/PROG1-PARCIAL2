using System;
using System.Collections;

using UnityEngine;

public class WaveController : MonoBehaviour
{
    [SerializeField] private float spawnDelay = 0f;
    [SerializeField] private float waveTime = 0f;
    [SerializeField] private Transform[] enemySpawns = null;

    private EnemyPoolController enemyPoolController = null;

    private int currentWave = 1;
    private float timer = 0f;

    private Action<int, int> onUpdateWave = null;

    private void Update()
    {
        UpdateTimer();
    }

    public void Init(EnemyPoolController enemyPoolController, Action<int, int> onUpdateWave)
    {
        this.enemyPoolController = enemyPoolController;
        this.onUpdateWave = onUpdateWave;

        UpdateWave(currentWave);
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

        int index = UnityEngine.Random.Range(0, enemySpawns.Length);
        return enemySpawns[index].position;
    }

    private void UpdateTimer()
    {
        timer += Time.deltaTime;
    }

    private void UpdateWave(int wave)
    {
        currentWave = wave;
        onUpdateWave?.Invoke(currentWave, 3);
    }
}
