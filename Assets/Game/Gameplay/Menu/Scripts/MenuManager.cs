using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Music Configuration")]
    [SerializeField] private AudioEvent musicEvent = null;
    [SerializeField] private AudioEvent playSoundEvent = null;

    [Header("Buttons Configuration")]
    [SerializeField] private Button playBtn = null;
    [SerializeField] private Button optionsBtn = null;
    [SerializeField] private Button creditsBtn = null;
    [SerializeField] private Button exitBtn = null;

    [Header("Panel Configuration")]
    [SerializeField] private GameObject optionsPanel = null;
    [SerializeField] private GameObject creditsPanel = null;
    [SerializeField] private GameObject tutorialPanel = null;

    [Header("Extra Buttons Configuration")]
    [SerializeField] private Button closeOptionsBtn = null;
    [SerializeField] private Button closeCreditsBtn = null;

    private PlayerInputActions inputAction = null;

    private void Start()
    {
        GameManager.Instance.AudioManager.PlayAudio(musicEvent);

        playBtn.onClick.AddListener(OpenTutorialPanel);
        optionsBtn.onClick.AddListener(() => ToggleOptionsPanel(true));
        creditsBtn.onClick.AddListener(() => ToggleOptionsCredits(true));
        exitBtn.onClick.AddListener(ExitGame);
        closeOptionsBtn.onClick.AddListener(() => ToggleOptionsPanel(false));
        closeCreditsBtn.onClick.AddListener(() => ToggleOptionsCredits(false));

        inputAction = new PlayerInputActions();
        inputAction.UI.Submit.performed += PlayGame;

        inputAction.UI.Submit.Disable();
    }

    private void PlayGame(InputAction.CallbackContext context)
    {
        GameManager.Instance.AudioManager.PlayAudio(playSoundEvent);
        GameManager.Instance.ChangeScene(SceneGame.Shooter);

        inputAction.UI.Submit.Disable();
    }

    private void OpenTutorialPanel()
    {
        tutorialPanel.SetActive(true);

        inputAction.UI.Submit.Enable();
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