using System;
using System.Collections;

using UnityEngine;

public class WaveController : MonoBehaviour
{
    [SerializeField] private WaveData[] waves = null;
    [SerializeField] private Transform[] enemySpawns = null;

    private EnemyPoolController enemyPoolController = null;

    private int currentWave = 0;
    private int waveEnemies = 0;

    private Action onPlayerWin = null;
    private Action<int, int> onUpdateWave = null;

    public void Init(EnemyPoolController enemyPoolController, Action onPlayerWin, Action<int, int> onUpdateWave)
    {
        this.enemyPoolController = enemyPoolController;
        this.onPlayerWin = onPlayerWin;
        this.onUpdateWave = onUpdateWave;

        UpdateWave(currentWave);
    }

    public void StartWave()
    {
        WaveData wave = waves[currentWave];
        waveEnemies = wave.EnemyCount;
        enemyPoolController.SetAvailableEnemies(wave.AvailableEnemies);

        StartCoroutine(SpawnEnemiesCoroutine());
        IEnumerator SpawnEnemiesCoroutine()
        {
            int spawnCount = 0;
            while (spawnCount < wave.EnemyCount)
            {
                yield return new WaitForSeconds(wave.SpawnDelay);

                enemyPoolController.SpawnEnemy(GetRandomSpawn());

                spawnCount++;
            }
        }
    }

    public void StopWave()
    {
        StopAllCoroutines();
    }

    public void OnKillEnemy()
    {
        waveEnemies--;
        if (waveEnemies <= 0)
        {
            if (currentWave < waves.Length - 1)
            {
                UpdateWave(currentWave + 1);
                StartWave();
            }
            else
            {
                onPlayerWin?.Invoke();
            }
        }
    }

    private Vector3 GetRandomSpawn()
    {
        if (enemySpawns.Length == 0) return transform.position;

        int index = UnityEngine.Random.Range(0, enemySpawns.Length);
        return enemySpawns[index].position;
    }

    private void UpdateWave(int wave)
    {
        currentWave = wave;
        onUpdateWave?.Invoke(currentWave + 1, waves.Length);
    }
}
