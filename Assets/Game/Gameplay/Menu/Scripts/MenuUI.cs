using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button playBtn = null;
    [SerializeField] private Button exitBtn = null;

    private void Start()
    {
        playBtn.onClick.AddListener(PlayGame);
        exitBtn.onClick.AddListener(ExitGame);
    }

    private void PlayGame()
    {
        GameManager.Get().ChangeScene(SceneGame.Shooter);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}