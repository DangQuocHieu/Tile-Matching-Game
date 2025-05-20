using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : PersistentSingleton<AudioManager>, IMessageHandle
{
    [Header("Audio Mixers")]
    [SerializeField] AudioMixer _audioMixer;

    [Header("SFX Audio")]
    [SerializeField] private Audio[] _sfxAudios;
    private Dictionary<AudioName, AudioSource> _sfxAudioSourcePool = new Dictionary<AudioName, AudioSource>();
    [Header("Music Audio")]
    [SerializeField] private Audio[] _musicAudios;
    private AudioSource _musicSource;

    [SerializeField] private Audio[] _matchComboAudios;
    private List<AudioSource> _matchComboSources = new List<AudioSource>();
    private float previousPlayTime = 0f;
    private float sfxCooldown = 0.08f;
    int _index = 0;

    protected override void Awake()
    {
        base.Awake();
        InitializeAudioSourcePool();
        InitializeMatchComboSourcePool();
        // ApplyAudioMixer();
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        MessageManager.AddSubscriber(GameMessageType.OnDiamondsCleared, this);
        MessageManager.AddSubscriber(GameMessageType.OnTurnStartDelay, this);
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        MessageManager.RemoveSubscriber(GameMessageType.OnDiamondsCleared, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnTurnStartDelay, this);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        switch (scene.name)
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

    private void InitializeMatchComboSourcePool()
    {
        foreach (var audio in _matchComboAudios)
        {
            AudioSource AudioSource = gameObject.AddComponent<AudioSource>();
            AudioSource.clip = audio.Clip;
            AudioSource.loop = false;
            AudioSource.volume = audio.Volume;
            AudioSource.pitch = audio.Pitch;
            AudioSource.priority = audio.Priority;
            _matchComboSources.Add(AudioSource);
        }
    }
    void InitializeAudioSourcePool()
    {
        _sfxAudioSourcePool = new Dictionary<AudioName, AudioSource>();
        foreach (var audio in _sfxAudios)
        {
            AudioSource AudioSource = gameObject.AddComponent<AudioSource>();
            AudioSource.clip = audio.Clip;
            AudioSource.loop = false;
            AudioSource.volume = audio.Volume;
            AudioSource.pitch = audio.Pitch;
            AudioSource.priority = audio.Priority;
            _sfxAudioSourcePool.Add(audio.AudioName, AudioSource);
        }
        _musicSource = gameObject.AddComponent<AudioSource>();
    }

    void ApplyAudioMixer()
    {
        if (_audioMixer == null) return;
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
        foreach (var audio in _musicAudios)
        {
            if (audio.AudioName == name)
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

    public void Handle(Message message)
    {
        switch (message.type)
        {
            case GameMessageType.OnDiamondsCleared:
                PlayMatchFX();
                break;
            case GameMessageType.OnTurnStartDelay:
                _index = 0;
                break;
        }
    }

    public void PlaySFX(AudioName name, bool isCooldown = false)
    {
        AudioSource source = _sfxAudioSourcePool[name];
        if (isCooldown)
        {
            float volumeMultiplier = Mathf.Clamp01((Time.time - previousPlayTime) / sfxCooldown);
            source.PlayOneShot(source.clip, source.volume * volumeMultiplier);
            previousPlayTime = Time.time;
        }
        else
        {
            if (source.isPlaying) source.Stop();
            source.PlayOneShot(source.clip);
        }
    }

    public void PlayMatchFX()
    {
        if (_index >= _matchComboAudios.Length)
        {
            _index = _matchComboAudios.Length - 1;
        }
        _matchComboSources[_index].PlayOneShot(_matchComboSources[_index].clip);
        ++_index;
    }

}
