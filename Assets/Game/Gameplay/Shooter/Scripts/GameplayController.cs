using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private AudioEvent musicEvent = null;
    [SerializeField] private Camera mainCamera = null;
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private WaveController waveController = null;
    [SerializeField] private EnemyPoolController enemyPoolController = null;
    [SerializeField] private RuneController runeController = null;

    public void Start()
    {
        playerController.Init(PlayerDefeat);
        runeController.Init(PlayerDefeat);
        waveController.Init(enemyPoolController);
        enemyPoolController.Init(mainCamera);
        enemyPoolController.SetMainTarget(runeController.transform);

        GameManager.Instance.AudioManager.PlayAudio(musicEvent);
    }

    private void PlayerDefeat()
    {
        playerController.UpdateInputFSM(FSM_INPUT.DISABLE_ALL);
        waveController.StopWave();
        enemyPoolController.EnemyList.ForEach(e => e.SetWinState());
    }
}
