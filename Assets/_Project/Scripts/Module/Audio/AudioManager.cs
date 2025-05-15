using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : PersistentSingleton<AudioManager>
{
    [Header("Audio Mixers")]
    [SerializeField] AudioMixer _audioMixer;
    
    [Header("SFX Audio")]
    [SerializeField] private Audio[] _sfxAudios;
    private Dictionary<AudioName, AudioSource> _sfxAudioSourcePool = new Dictionary<AudioName, AudioSource>();
    [Header("Music Audio")]
    [SerializeField] private Audio[] _musicAudios;
    private AudioSource _musicSource;


    protected override void Awake()
    {
        base.Awake();
        InitializeAudioSourcePool();
        // ApplyAudioMixer();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        switch(scene.name)
        {
            case "Menu Scene":
                PlayMusic(AudioName.MenuMusic);
                break;
            case "Gameplay Scene":
                PlayMusic(AudioName.GamePlayMusic);
                break;
            case "Character Selection Scene":
                PlayMusic(AudioName.CharacterSelectionMusic);
                break;
        }
    }
    void InitializeAudioSourcePool()
    {
        _sfxAudioSourcePool = new Dictionary<AudioName, AudioSource>();
        foreach(var audio in _sfxAudios)
        {
            AudioSource AudioSource = gameObject.AddComponent<AudioSource>();
            AudioSource.clip = audio.Clip;
            AudioSource.loop = false;
            AudioSource.volume = audio.Volume;
            AudioSource.pitch = audio.Pitch;
            AudioSource.priority = audio.Priority;
            _sfxAudioSourcePool.Add(audio.AudioName, AudioSource);
            break;   
        }
        _musicSource = gameObject.AddComponent<AudioSource>();
    }

    void ApplyAudioMixer()
    {
        if(_audioMixer == null) return;
        foreach (var source in _sfxAudioSourcePool.Values)
        {
            source.outputAudioMixerGroup = _audioMixer.FindMatchingGroups("SFX")[0];
        }
        _musicSource.outputAudioMixerGroup = _audioMixer.FindMatchingGroups("Music")[0]; 
    }

    public void PlayMusic(AudioName name)
    {
        if (_musicSource.isPlaying)
        {
            _musicSource.Stop();
        }
        _musicSource.UnPause();
        foreach(var audio in _musicAudios)
        {
            if(audio.AudioName == name)
            {
                _musicSource.clip = audio.Clip;
                _musicSource.volume = audio.Volume;
                _musicSource.pitch = audio.Pitch;
                _musicSource.loop = true;
                _musicSource.priority = audio.Priority;
            }
        }
        AudioListener.pause = false;
        _musicSource.Play();
    }

    public void PauseMusic(object[] datas)
    {
        _musicSource.Pause();
    }

    public void ContinuePlayMusic(object[] datas)
    {
        _musicSource.UnPause();
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }
}
