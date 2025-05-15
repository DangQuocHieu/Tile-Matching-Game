using DG.Tweening;
using UnityEngine;
[CreateAssetMenu(fileName = "MoveToAnimationSO", menuName = "Scriptable Objects/DiamondAnimation/MoveToAnimationSO")]
public class MoveToAnimationSO : ScriptableObject
{
    [SerializeField] private float _duration;
    [SerializeField] private Ease _ease;

    public void MoveTo(Transform fromTransform, Vector3 position, TweenCallback callback)
    {
        fromTransform.DOMove(position, _duration).SetEase(_ease).OnComplete(()=>{
            callback?.Invoke();
        });
    }
}
