using System;
using UnityEngine;

public class GameAudioSource : MonoBehaviour
{
    private AudioSource _audioSource;
    private SfxInfo _sfxInfo;

    private Action<GameAudioSource> _onKillCallback;

    private void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if(!_audioSource.isPlaying)
        {
            KillSelf();
        }
    }

    public void Setup(SfxInfo sfxInfo, float globalSFXVolume)
    {
        _sfxInfo = sfxInfo;
        _audioSource.clip = sfxInfo._clip;
        _audioSource.volume = sfxInfo._volume * globalSFXVolume;
        _audioSource.pitch = sfxInfo._pitch;
    }

    public void Play()
    {
        _audioSource.Play();
    }

    public void SetVolume(float globalSfxVolume)
    {
        _audioSource.volume = _sfxInfo._volume * globalSfxVolume;
    }

    public GameAudioSource SetOnKillCallback(Action<GameAudioSource> onKillCallback)
    {
        _onKillCallback = onKillCallback;

        return this;
    }
    
    private void KillSelf()
    {
        _onKillCallback?.Invoke(this);

        Destroy(_audioSource);
        Destroy(this);
    }
}
