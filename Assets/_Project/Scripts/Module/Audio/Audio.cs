using UnityEngine;
using UnityEngine.Audio;

public enum AudioName
{
    MenuMusic,
    GamePlayMusic,
    CharacterSelectionMusic,
}

[System.Serializable]
public class Audio
{
    public AudioName AudioName;
    public AudioClip Clip;
    [Range(0f, 1f)] public float Volume = 1f;
    [Range(-3f, 3f)] public float Pitch = 1f;
    [Range(0f, 256f)] public int Priority = 128;
}