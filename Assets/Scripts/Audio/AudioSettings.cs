using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ CreateAssetMenu (fileName = nameof(AudioSettings), menuName = "Audio/Audio settings")]
public class AudioSettings : ScriptableObject
{
    [SerializeField] private SfxInfo[] _soundsFX;
    [SerializeField] private AudioClip[] _musicTracks;

    private int _currentMusicTrackNumber;

    private readonly Dictionary<SfxType, SfxInfo> _soundsFXMap = new Dictionary<SfxType, SfxInfo>();

    private void OnEnable()
    {
        FillMap();
    }

    private void OnValidate()
    {
        if (_soundsFX == null)
            return;

        foreach (SfxInfo sfxInfo in _soundsFX)
        {
            sfxInfo.UpdateName();
        }
    }

    public AudioClip GetAudioClip(SfxType type)
    {
        return _soundsFXMap.TryGetValue(type, out SfxInfo value) ? value._clip : null;
    }

    public SfxInfo GetSfxInfo(SfxType type)
    {
        return _soundsFXMap.TryGetValue(type, out SfxInfo info) ? info : null;
    }

    public AudioClip GetRandomMusicTrack()
    {
        if ((_musicTracks == null) || (_musicTracks.Length == 0)) 
            return null;

        _currentMusicTrackNumber = Random.Range(0, _musicTracks.Length);
        return _musicTracks[_currentMusicTrackNumber];
    }

    public AudioClip GetNextMusicTrack()
    {
        if ((_musicTracks == null) || (_musicTracks.Length == 0))
            return null;

        _currentMusicTrackNumber++;
        if (_currentMusicTrackNumber >= _musicTracks.Length)
            _currentMusicTrackNumber = 0;

        return _musicTracks[_currentMusicTrackNumber];
    }
    
    private void FillMap()
    {
        _soundsFXMap.Clear();

        if (_soundsFX == null)
            return;

        foreach (SfxInfo sfxInfo in _soundsFX)
        {
            SfxType type = sfxInfo._type;
            if(!_soundsFXMap.ContainsKey(type))
            {
                _soundsFXMap.Add(type, sfxInfo);
            }
        }
    }
}
