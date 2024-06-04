using UnityEngine;

public class AudioSfx : MonoBehaviour
{
    private AudioSource audioSource = null;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetAudioValues(AudioEvent audioEvent)
    {
        audioSource.volume = audioEvent.Volume;
        audioSource.pitch = audioEvent.Pitch;
        audioSource.PlayOneShot(audioEvent.Clip);
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }

    public void OnGet()
    {
        gameObject.SetActive(true);
        audioSource.enabled = true;
    }

    public void OnRelease()
    {
        audioSource.enabled = false;
        gameObject.SetActive(false);
    }
}