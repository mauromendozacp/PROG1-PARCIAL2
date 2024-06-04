using System;
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
    private bool isStopingMusic = false;

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

        sfxEnabled = true;
        musicEnabled = true;
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

    private void PlaySFX(AudioEvent audioEvent)
    {
        if (!sfxEnabled) return;

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

    private void PlayMusic(AudioEvent audioEvent)
    {
        if (!musicEnabled) return;

        StopCurrentMusic(onSuccess: () => { StartCoroutine(StartMusicCoroutine()); });
        IEnumerator StartMusicCoroutine()
        {
            currentMusicEvent = audioEvent;
            musicAudioSource.clip = audioEvent.Clip;
            musicAudioSource.volume = audioEvent.Volume;
            musicAudioSource.Play();

            float timer = 0f;
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

    public void StopCurrentMusic(Action onSuccess = null)
    {
        StartCoroutine(StopMusicCoroutine());
        IEnumerator StopMusicCoroutine()
        {
            if (currentMusicEvent == null || isStopingMusic)
            {
                yield return new WaitUntil(predicate: () => !isStopingMusic);
            }
            else
            {
                float timer = 0f;
                isStopingMusic = true;
                
                while (timer < musicLerpTime)
                {
                    timer += Time.deltaTime;

                    float musicVolume = Mathf.Lerp(currentMusicEvent.Volume, defaultMinMixerVolume, timer / musicLerpTime);
                    audioMixerGroupsDic[musicMixerName].audioMixer.SetFloat(musicVolumeParameter, musicVolume);

                    yield return new WaitForEndOfFrame();
                }
                audioMixerGroupsDic[musicMixerName].audioMixer.SetFloat(musicVolumeParameter, defaultMinMixerVolume);

                currentMusicEvent = null;
                musicAudioSource.Stop();
                isStopingMusic = false;
            }

            onSuccess?.Invoke();
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
