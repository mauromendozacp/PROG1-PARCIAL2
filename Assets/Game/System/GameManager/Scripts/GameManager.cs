﻿using System;

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

    public override void Awake()
    {
        if (instance == null)
        {
            loadingManager.LoadingScene(SceneGame.Loading);
        }

        base.Awake();
    }

    public void ChangeScene(SceneGame nextScene, Action onComplete = null)
    {
        audioManager.StopCurrentMusic();
        loadingManager.TransitionScene(nextScene, onComplete);
    }
}