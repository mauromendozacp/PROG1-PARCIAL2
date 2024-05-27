using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [Header("Reference Settings"), Space]
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private WaveController waveController = null;
    [SerializeField] private EnemyPoolController enemyPoolController = null;

    public void Start()
    {
        playerController.Init(PlayerDefeat);
        waveController.Init(enemyPoolController);
    }

    private void PlayerDefeat()
    {
        waveController.StopWave();
        enemyPoolController.EnemyList.ForEach(e => e.SetWinState());
    }
}
