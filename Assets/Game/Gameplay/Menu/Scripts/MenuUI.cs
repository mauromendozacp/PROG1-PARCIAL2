using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [Header("Main Buttons Configurations")]
    [SerializeField] private Button playBtn = null;
    [SerializeField] private Button optionsBtn = null;
    [SerializeField] private Button creditsBtn = null;
    [SerializeField] private Button exitBtn = null;

    [Header("Extra Panel Configurations")]
    [SerializeField] private GameObject optionsPanel = null;
    [SerializeField] private GameObject creditsPanel = null;

    [Header("Extra Buttons Configurations")]
    [SerializeField] private Button closeOptionsBtn = null;
    [SerializeField] private Button closeCreditsBtn = null;

    private void Start()
    {
        playBtn.onClick.AddListener(PlayGame);
        optionsBtn.onClick.AddListener(() => ToggleOptionsPanel(true));
        creditsBtn.onClick.AddListener(() => ToggleOptionsCredits(true));
        exitBtn.onClick.AddListener(ExitGame);
        closeOptionsBtn.onClick.AddListener(() => ToggleOptionsPanel(false));
        closeCreditsBtn.onClick.AddListener(() => ToggleOptionsCredits(false));
    }

    private void PlayGame()
    {
        GameManager.Get().ChangeScene(SceneGame.Shooter);
    }

    private void ToggleOptionsPanel(bool status)
    {
        optionsPanel.SetActive(status);
    }

    private void ToggleOptionsCredits(bool status)
    {
        creditsPanel.SetActive(status);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}