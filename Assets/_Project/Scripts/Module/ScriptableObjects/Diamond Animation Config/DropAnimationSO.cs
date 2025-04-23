using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "DropAnimationSO", menuName = "Scriptable Objects/DropAnimationSO")]
public class DropAnimationSO : ScriptableObject
{
    [SerializeField] private float _duration;
    [SerializeField] private Ease _ease;

    public Tween Drop(GameObject gameObject, int yPosition, TweenCallback callback = null)
    {
        return gameObject.transform.DOMoveY(yPosition, _duration).SetEase(_ease).OnComplete(()=>{callback?.Invoke();});
    }
}
