using UnityEngine;

[CreateAssetMenu(fileName = "Wave_", menuName = "ScriptableObjects/Wave", order = 1)]
public class WaveData : ScriptableObject
{
    [SerializeField] private ENEMY_TYPE[] availableEnemies = null;
    [SerializeField] private int enemyCount = 0;
    [SerializeField] private float spawnDelay = 0f;
    [SerializeField] private float startWaveDelay = 0f;

    public ENEMY_TYPE[] AvailableEnemies { get => availableEnemies; }
    public int EnemyCount { get => enemyCount; }
    public float SpawnDelay { get => spawnDelay; }
    public float StartWaveDelay { get => startWaveDelay; }
}
