using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class GameplayUI : MonoBehaviour
{
    [Header("HUD Settings")]
    [SerializeField] private Slider playerHealthSlider = null;
    [SerializeField] private Slider runeHealthSlider = null;
    [SerializeField] private TMP_Text waveText = null;

    [Header("Pause Settings")]
    [SerializeField] private GameObject pausePanel = null;
    [SerializeField] private Button resumeBtn = null;
    [SerializeField] private Button backToMenuBtn = null;

    private Action onEnablePlayerInput = null;

    private const string allWaveText = "Wave {0, 0}/{1, 0}";

    private void Start()
    {
        resumeBtn.onClick.AddListener(() => TogglePause(false));
        backToMenuBtn.onClick.AddListener(BackToMenu);
    }

    public void Init(Action onEnablePlayerInput)
    {
        this.onEnablePlayerInput = onEnablePlayerInput;
    }

    public void UpdatePlayerHealth(int currentLives, int maxLives)
    {
        playerHealthSlider.value = (float)currentLives / maxLives;
    }

    public void UpdateRuneHealth(int currentLives, int maxLives)
    {
        runeHealthSlider.value = (float)currentLives / maxLives;
    }

    public void UpdateWave(int currentWave, int maxWave)
    {
        waveText.text = string.Format(allWaveText, currentWave, maxWave);
    }

    public void TogglePause(bool status)
    {
        pausePanel.SetActive(status);
        ToggleTimeScale(!status);

        if (!status)
        {
            onEnablePlayerInput?.Invoke();
        }
    }

    private void BackToMenu()
    {
        resumeBtn.interactable = false;
        backToMenuBtn.interactable = false;

        GameManager.Instance.ChangeScene(SceneGame.Menu);
        ToggleTimeScale(true);
    }

    private void ToggleTimeScale(bool status)
    {
        Time.timeScale = status ? 1f : 0f;
    }
}