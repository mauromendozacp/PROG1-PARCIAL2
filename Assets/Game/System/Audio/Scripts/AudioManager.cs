using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class AudioManager : MonoBehaviour
{
    [Header("Main Configuration")]
    [SerializeField] private AudioMixer audioMixer = null;
    [SerializeField] private AudioSource musicAudioSource = null;
    [SerializeField] private Transform sfxHolder = null;
    [SerializeField] private AudioSfx audioSfxPrefab = null;

    [Header("Lerp Times Configuration")]
    [SerializeField] private float musicLerpTime = 1.5f;

    private ObjectPool<AudioSfx> sfxAudioSourcesPool = null;
    private List<AudioSfx> activeSfxAudioSources = null;
    private Dictionary<string, AudioMixerGroup> audioMixerGroupsDic = null;

    private bool sfxEnabled = false;
    private bool musicEnabled = false;

    private AudioEvent currentMusicEvent = null;

    public const string masterMixerName = "Master";
    public const string musicMixerName = "Music";
    public const string musicVolumeParameter = "musicVolume";

    public const float defaultMaxMixerVolume = 0.0f;
    public const float defaultMinMixerVolume = -40.0f;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        audioMixerGroupsDic = new Dictionary<string, AudioMixerGroup>();
        AudioMixerGroup[] groups = audioMixer.FindMatchingGroups(masterMixerName);
        for (int i = 0; i < groups.Length; i++)
        {
            audioMixerGroupsDic.Add(groups[i].name, groups[i]);
        }

        sfxAudioSourcesPool = new ObjectPool<AudioSfx>(GenerateSFXSource, GetSFXSource, ReleaseSFXSource);
        activeSfxAudioSources = new List<AudioSfx>();
    }

    public void PlayAudio(AudioEvent audioEvent)
    {
        if (audioEvent == null) return;

        switch (audioEvent.AudioType)
        {
            case AudioEvent.AUDIO_TYPE.SFX:
                PlaySFX(audioEvent);
                break;

            case AudioEvent.AUDIO_TYPE.MUSIC:
                PlayMusic(audioEvent);
                break;
        }
    }

    public void PlaySFX(AudioEvent audioEvent)
    {
        if (sfxEnabled)
        {
            AudioSfx audio = sfxAudioSourcesPool.Get();
            audio.SetAudioValues(audioEvent);

            ToggleSFX(sfxEnabled);

            IEnumerator ReleaseSource()
            {
                yield return new WaitForSecondsRealtime(audioEvent.Clip.length);
                sfxAudioSourcesPool.Release(audio);
            }

            StartCoroutine(ReleaseSource());
        }
    }

    public void PlayMusic(AudioEvent audioEvent)
    {
        StartCoroutine(TransitionMusicCoroutine());
        IEnumerator TransitionMusicCoroutine()
        {
            float timer = 0f;

            if (currentMusicEvent != null)
            {
                while (timer < musicLerpTime)
                {
                    timer += Time.deltaTime;

                    float musicVolume = Mathf.Lerp(currentMusicEvent.Volume, defaultMinMixerVolume, timer / musicLerpTime);
                    audioMixerGroupsDic[musicMixerName].audioMixer.SetFloat(musicVolumeParameter, musicVolume);

                    yield return new WaitForEndOfFrame();
                }
            }

            audioMixerGroupsDic[musicMixerName].audioMixer.SetFloat(musicVolumeParameter, defaultMinMixerVolume);
            musicAudioSource.clip = audioEvent.Clip;
            musicAudioSource.volume = audioEvent.Volume;
            currentMusicEvent = audioEvent;

            timer = 0f;
            while (timer < musicLerpTime)
            {
                timer += Time.deltaTime;

                float musicVolume = Mathf.Lerp(defaultMinMixerVolume, defaultMaxMixerVolume, timer / musicLerpTime);
                audioMixerGroupsDic[musicMixerName].audioMixer.SetFloat(musicVolumeParameter, musicVolume);

                yield return new WaitForEndOfFrame();
            }

            audioMixerGroupsDic[musicMixerName].audioMixer.SetFloat(musicVolumeParameter, defaultMaxMixerVolume);
        }
    }

    public void ToggleMusic(bool state)
    {
        musicEnabled = state;
        if (musicEnabled)
        {
            if (!musicAudioSource.isPlaying)
            {
                musicAudioSource.Play();
            }
        }
        else
        {
            musicAudioSource.Pause();
        }
    }

    public void ToggleSFX(bool status)
    {
        if (!status)
        {
            for (int i = 0; i < activeSfxAudioSources.Count; i++)
            {
                activeSfxAudioSources[i].StopAudio();
            }
        }

        sfxEnabled = status;
    }

    private AudioSfx GenerateSFXSource()
    {
        return Instantiate(audioSfxPrefab, sfxHolder);
    }

    private void GetSFXSource(AudioSfx audio)
    {
        activeSfxAudioSources.Add(audio);
        audio.OnGet();
    }

    private void ReleaseSFXSource(AudioSfx audio)
    {
        activeSfxAudioSources.Remove(audio);
        audio.OnRelease();
    }
}
