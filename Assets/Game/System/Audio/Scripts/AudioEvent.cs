using UnityEngine;

[CreateAssetMenu(fileName = "AudioEvent_", menuName = "ScriptableObjects/Audio/AudioEvent", order = 1)]
public class AudioEvent : ScriptableObject
{
    public enum AUDIO_TYPE
    {
        SFX,
        MUSIC
    }

    public enum SOUND_TYPE
    {
        MONO,
        STEREO
    }

    [SerializeField] private AUDIO_TYPE audioType = default;
    [SerializeField] private SOUND_TYPE soundType = default;
    [SerializeField] private AudioClip clip = null;
    [SerializeField] private bool useRandomClip = false;
    [SerializeField] private AudioClip[] clips = null;

    [SerializeField] [Range(0, 1)] private float volume = 1.0f;

    [SerializeField] private float pitch = 1.0f;
    [SerializeField] private bool useRandomPitch = false;
    [SerializeField] private float minPitch = 0.0f;
    [SerializeField] private float maxPitch = 1.0f;

    public AUDIO_TYPE AudioType { get => audioType; }
    public SOUND_TYPE SoundType { get => soundType; }
    public AudioClip Clip { get => useRandomClip ? clips[Random.Range(0, clips.Length)] : clip; }
    public float Volume { get => volume; }
    public float Pitch { get => useRandomPitch ? Random.Range(minPitch, maxPitch) : pitch; }
}
