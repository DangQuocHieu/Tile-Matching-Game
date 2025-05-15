using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectController : Singleton<ParticleEffectController>, IMessageHandle
{
    [SerializeField] private ParticleSystem _healFX;
    [SerializeField] private ParticleSystem _magicPointFX;
    [SerializeField] private ParticleSystem _rageFX;
    [SerializeField] private ParticleSystem _shieldFX;
    private Dictionary<DiamondType, ParticleSystem> _particleEffectDictionary = new Dictionary<DiamondType, ParticleSystem>();

    void Start()
    {
        InitializeDictionary();
    }

    void OnEnable()
    {
        MessageManager.AddSubscriber(GameMessageType.OnApplyEffectStart, this);
        MessageManager.AddSubscriber(GameMessageType.OnApplyGainCard, this);
    }

    void OnDisable()
    {
        MessageManager.RemoveSubscriber(GameMessageType.OnApplyEffectStart, this);
        MessageManager.RemoveSubscriber(GameMessageType.OnApplyGainCard, this);
    }

    private void InitializeDictionary()
    {

        _particleEffectDictionary.Add(DiamondType.Health, _healFX);
        _particleEffectDictionary.Add(DiamondType.MagicPoint, _magicPointFX);
        _particleEffectDictionary.Add(DiamondType.Rage, _rageFX);
        _particleEffectDictionary.Add(DiamondType.Shield, _shieldFX);
    }

    public void Handle(Message message)
    {
        switch (message.type)
        {
            case GameMessageType.OnApplyEffectStart:
                {
                    DiamondType type = (DiamondType)message.data[0];
                    PlayFX(type);
                    break;
                }

            case GameMessageType.OnApplyGainCard:
                {
                    DiamondType type = (DiamondType)message.data[0];
                    PlayFX(type);
                    break;
                }
        }
    }

    public void PlayFX(DiamondType type)
    {
        if (_particleEffectDictionary.ContainsKey(type))
        {
            ParticleSystem fx = _particleEffectDictionary[type];
            fx.transform.position = BattleManager.Instance.CurrentUnit.transform.position;
            fx.Play();
        }
    }
}
