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

    [Header("Pause Settings")]
    [SerializeField] private GameObject losePanel = null;
    [SerializeField] private Button retryBtn = null;
    [SerializeField] private Button loseBackToMenuBtn = null;

    [Header("Pause Settings")]
    [SerializeField] private GameObject winPanel = null;
    [SerializeField] private Button winBackToMenuBtn = null;

    private Action onEnablePlayerInput = null;

    private const string allWaveText = "Wave {0, 0}/{1, 0}";

    private void Start()
    {
        resumeBtn.onClick.AddListener(() => TogglePause(false));
        backToMenuBtn.onClick.AddListener(BackToMenu);

        retryBtn.onClick.AddListener(Retry);
        loseBackToMenuBtn.onClick.AddListener(BackToMenu);

        winBackToMenuBtn.onClick.AddListener(BackToMenu);
    }

    public void Init(Action onEnablePlayerInput)
    {
        this.onEnablePlayerInput = onEnablePlayerInput;
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

    public void OpenLosePanel()
    {
        losePanel.SetActive(true);
    }

    public void OpenWinPanel()
    {
        winPanel.SetActive(true);
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
    
    private void Retry()
    {
        retryBtn.interactable = false;
        loseBackToMenuBtn.interactable = false;

        GameManager.Instance.ChangeScene(SceneGame.Shooter);
        GameManager.Instance.AudioManager.ToggleMusic(true);
    }

    private void BackToMenu()
    {
        resumeBtn.interactable = false;
        backToMenuBtn.interactable = false;

        retryBtn.interactable = false;
        loseBackToMenuBtn.interactable = false;

        winBackToMenuBtn.interactable = false;

        GameManager.Instance.ChangeScene(SceneGame.Menu);
        GameManager.Instance.AudioManager.ToggleMusic(true);
        ToggleTimeScale(true);
    }

    private void ToggleTimeScale(bool status)
    {
        Time.timeScale = status ? 1f : 0f;
    }
}