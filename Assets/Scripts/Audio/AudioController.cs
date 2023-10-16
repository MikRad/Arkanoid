using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : SingletonMonoBehaviour<AudioController>
{
    [SerializeField] private AudioSettings _settings;
    
    private const string MusicVolumeKey = "ArkanoidMusicVolumeKey";
    private const string SfxVolumeKey = "ArkanoidSfxVolumeKey";

    private AudioSource _musicTrackSource;

    private float _musicVolume = 1f;
    private float _sfxVolume = 1f;

    private readonly List<GameAudioSource> _sfxSources = new List<GameAudioSource>();

    public float MusicVolume { get => _musicVolume; set{ _musicVolume = value; _musicTrackSource.volume = _musicVolume; } }
    public float SfxVolume
    {
        get => _sfxVolume; 
        set
        {
            _sfxVolume = value; 
            foreach (GameAudioSource sfxSource in _sfxSources)
            {
                sfxSource.SetVolume(_sfxVolume);
            }
        } 
    }

    /*protected override void Awake()
    {
        base.Awake();
        
        ReadSoundParams();

        _musicTrackSource = GetComponent<AudioSource>();
        _musicTrackSource.volume = _musicVolume;
        _musicTrackSource.loop = false;
        
        PlayRandomTrack();
    }

    private void OnDestroy()
    {
        SaveSoundParams();
    }

    private void Update()
    {
        if (_musicTrackSource.isPlaying)
            return;

        PlayNextTrack();
    }*/

    public void PlaySfx(SfxType sfxType, Transform targetTransform = null)
    {
        if ((targetTransform != null) && (!targetTransform.gameObject.activeSelf))
        {
            Debug.LogError("Target gameobject for Sfx is not active!");
            return;
        }

        GameAudioSource audioSrc = CreateAudioSource(targetTransform);
        SetupAudioSource(audioSrc, _settings.GetSfxInfo(sfxType));
    }

    private GameAudioSource CreateAudioSource(Transform targetTransform)
    {
        Transform transformForAudioSrc = (targetTransform == null) ? transform : targetTransform;
        GameAudioSource audioSrc = transformForAudioSrc.gameObject.AddComponent<GameAudioSource>();
        audioSrc.SetOnKillCallback(OnAudioSourceKilled);
        _sfxSources.Add(audioSrc);

        return audioSrc;
    }

    private void OnAudioSourceKilled(GameAudioSource aSource)
    {
        _sfxSources.Remove(aSource);
    }

    private void SetupAudioSource(GameAudioSource audioSrc, SfxInfo sfxInfo)
    {
        audioSrc.Setup(sfxInfo, _sfxVolume);
        audioSrc.Play();
    }

    private AudioClip GetAudioClip(SfxType type)
    {
        return _settings.GetAudioClip(type);
    }

    private void PlayRandomTrack()
    {
        _musicTrackSource.clip = _settings.GetRandomMusicTrack();
        _musicTrackSource.Play();
    }

    private void PlayNextTrack()
    {
        _musicTrackSource.clip = _settings.GetNextMusicTrack();
        _musicTrackSource.Play();
    }

    private void ReadSoundParams()
    {
        if (PlayerPrefs.HasKey(MusicVolumeKey))
            _musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey);
        if (PlayerPrefs.HasKey(SfxVolumeKey))
            _sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey);
    }

    private void SaveSoundParams()
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, _musicVolume);
        PlayerPrefs.SetFloat(SfxVolumeKey, _sfxVolume);
    }
}
