using UnityEngine;

public enum SceneGame
{
    Menu,
    Shooter,
    Loading
}

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [Header("Manager References")]
    [SerializeField] private LoadingManager loadingManager = null;
    [SerializeField] private AudioManager audioManager = null;

    public LoadingManager LoadingManager => loadingManager;
    public AudioManager AudioManager => audioManager;

    private void Start()
    {
        loadingManager.LoadingScene(SceneGame.Loading);
    }

    public void ChangeScene(SceneGame nextScene)
    {
        loadingManager.TransitionScene(nextScene);
    }
}