using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private Slider playerHealthSlider = null;
    [SerializeField] private Slider runeHealthSlider = null;
    [SerializeField] private TMP_Text waveText = null;

    private const string allWaveText = "Wave {0, 0}/{1, 0}";

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
}