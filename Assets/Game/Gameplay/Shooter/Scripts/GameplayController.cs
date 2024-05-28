using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [Header("Reference Settings"), Space]
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private WaveController waveController = null;
    [SerializeField] private EnemyPoolController enemyPoolController = null;
    [SerializeField] private RuneController runeController = null;

    public void Start()
    {
        playerController.Init(PlayerDefeat);
        runeController.Init(PlayerDefeat);
        waveController.Init(enemyPoolController, runeController.transform);
    }

    private void PlayerDefeat()
    {
        playerController.UpdateInputFSM(FSM_INPUT.DISABLE_ALL);
        waveController.StopWave();
        enemyPoolController.EnemyList.ForEach(e => e.SetWinState());
    }
}
