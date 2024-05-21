using System;
using UnityEngine.SceneManagement;

public enum SceneGame
{
    Menu,
    Shooter
}

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public void ChangeScene(SceneGame scene)
    {
        string sceneName;

        switch (scene)
        {
            case SceneGame.Menu:
                sceneName = "Menu";
                break;
            case SceneGame.Shooter:
                sceneName = "Shooter";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(scene), scene, null);
        }

        SceneManager.LoadScene(sceneName);
    }
}