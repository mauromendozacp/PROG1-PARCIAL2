using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private AudioEvent musicEvent = null;
    [SerializeField] private Camera mainCamera = null;
    [SerializeField] private GameplayUI gameplayUI = null;

    [Header("Controllers Settings")]
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private WaveController waveController = null;
    [SerializeField] private EnemyPoolController enemyPoolController = null;
    [SerializeField] private RuneController runeController = null;

    public void Start()
    {
        playerController.Init(gameplayUI.UpdatePlayerHealth, PlayerDefeat, onPause: () => gameplayUI.TogglePause(true));
        runeController.Init(gameplayUI.UpdateRuneHealth, PlayerDefeat);
        waveController.Init(enemyPoolController, gameplayUI.UpdateWave);
        enemyPoolController.Init(mainCamera, waveController.OnKillEnemy);
        enemyPoolController.SetMainTarget(runeController.transform);

        GameManager.Instance.AudioManager.PlayAudio(musicEvent);

        waveController.StartWave();
    }

    private void PlayerDefeat()
    {
        playerController.PlayerDefeat();
        waveController.StopWave();
        enemyPoolController.EnemyList.ForEach(e => e.SetWinState());
    }
}
