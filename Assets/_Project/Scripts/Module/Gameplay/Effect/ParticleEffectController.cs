using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectController : Singleton<ParticleEffectController>, IMessageHandle
{
    [SerializeField] private ParticleSystem _healFX;
    [SerializeField] private ParticleSystem _manaFX;
    [SerializeField] private ParticleSystem _rageFX;
    [SerializeField] private Vector3 _leftPosition = new Vector3(-4, 2, 0);
    [SerializeField] private Vector3 _rightPosition = new Vector3(11, 2, 0);
    private Dictionary<Side, Vector3> _effectPosition = new Dictionary<Side, Vector3>();
    private Dictionary<DiamondType, ParticleSystem> _particleEffectDictionary = new Dictionary<DiamondType, ParticleSystem>();

    void Start()
    {
        InitializeDictionary();
    }

    void OnEnable()
    {
        MessageManager.AddSubcriber(GameMessageType.OnApplyEffect, this);
    }

    void OnDisable()
    {
        MessageManager.RemoveSubcriber(GameMessageType.OnApplyEffect, this);
    }

    private void InitializeDictionary()
    {
        _effectPosition.Add(Side.LeftSide, _leftPosition);
        _effectPosition.Add(Side.RightSide, _rightPosition);

        _particleEffectDictionary.Add(DiamondType.Health, _healFX);
        _particleEffectDictionary.Add(DiamondType.Mana, _manaFX);
        _particleEffectDictionary.Add(DiamondType.Rage, _rageFX);
    }

    public void Handle(Message message)
    {
        switch (message.type)
        {
            case GameMessageType.OnApplyEffect:
                DiamondType type = (DiamondType)message.data[0];
                PlayFX(type);
                break;
        }
    }

    public void PlayFX(DiamondType type)
    {
        if (_particleEffectDictionary.ContainsKey(type))
        {
            ParticleSystem fx = _particleEffectDictionary[type];
            Side currentSide = TurnManager.Instance.CurrentSide;
            fx.transform.position = _effectPosition[currentSide];
            fx.Play();
        }
    }
}
