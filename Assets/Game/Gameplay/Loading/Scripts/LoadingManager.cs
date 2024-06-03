using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    private LoadingUI loadingUI = null;

    private static readonly Dictionary<SceneGame, string> sceneNames = new Dictionary<SceneGame, string>()
    {
        { SceneGame.Menu, "Menu" },
        { SceneGame.Shooter, "Shooter" },
        { SceneGame.Loading, "Loading" }
    };

    public void SetLoadingUI(LoadingUI loadingUI)
    {
        this.loadingUI = loadingUI;
    }

    public void TransitionScene(SceneGame nextScene, Action onComplete = null)
    {
        loadingUI.ToggleUI(true,
            onComplete: () =>
            {
                UnloadScene(GetCurrentScene(),
                    onSuccess: () =>
                    {
                        LoadingScene(nextScene,
                            onSuccess: () =>
                            {
                                loadingUI.ToggleUI(false, onComplete);
                            });
                    });
            });
    }

    public void LoadingScene(SceneGame scene, Action onSuccess = null)
    {
        if (sceneNames.TryGetValue(scene, out string sceneName))
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            op.completed += (op) =>
            {
                onSuccess?.Invoke();
            };
        }
    }

    private void UnloadScene(SceneGame scene, Action onSuccess = null)
    {
        if (sceneNames.TryGetValue(scene, out string sceneName))
        {
            AsyncOperation op = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(sceneName));
            op.completed += (op) =>
            {
                onSuccess?.Invoke();
            };
        }
    }

    private SceneGame GetCurrentScene()
    {
        string currSceneName = SceneManager.GetActiveScene().name;

        foreach (KeyValuePair<SceneGame, string> scene in sceneNames)
        {
            if (scene.Value == currSceneName)
            {
                return scene.Key;
            }
        }

        return default;
    }
}
