using System;
using UnityEngine.SceneManagement;

public enum SceneGame
{
    Mainmenu,
    Gameplay
}

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public void ChangeScene(SceneGame scene)
    {
        string sceneName;

        switch (scene)
        {
            case SceneGame.Mainmenu:
                sceneName = "MainMenu";
                break;
            case SceneGame.Gameplay:
                sceneName = "Gameplay";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(scene), scene, null);
        }

        SceneManager.LoadScene(sceneName);
    }
}