using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Main Configuration")]
    [SerializeField] private AudioEvent musicEvent = null;

    [Header("Buttons Configuration")]
    [SerializeField] private Button playBtn = null;
    [SerializeField] private Button optionsBtn = null;
    [SerializeField] private Button creditsBtn = null;
    [SerializeField] private Button exitBtn = null;

    [Header("Extra Panel Configuration")]
    [SerializeField] private GameObject optionsPanel = null;
    [SerializeField] private GameObject creditsPanel = null;

    [Header("Extra Buttons Configuration")]
    [SerializeField] private Button closeOptionsBtn = null;
    [SerializeField] private Button closeCreditsBtn = null;

    private void Start()
    {
        GameManager.Instance.AudioManager.PlayAudio(musicEvent);

        playBtn.onClick.AddListener(PlayGame);
        optionsBtn.onClick.AddListener(() => ToggleOptionsPanel(true));
        creditsBtn.onClick.AddListener(() => ToggleOptionsCredits(true));
        exitBtn.onClick.AddListener(ExitGame);
        closeOptionsBtn.onClick.AddListener(() => ToggleOptionsPanel(false));
        closeCreditsBtn.onClick.AddListener(() => ToggleOptionsCredits(false));
    }

    private void PlayGame()
    {
        GameManager.Instance.ChangeScene(SceneGame.Shooter);
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