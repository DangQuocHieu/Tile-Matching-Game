using System.Collections;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "ScaleAnimationSO", menuName = "Scriptable Objects/ScaleAnimationSO")]
public class ScaleAmimationSO : ScriptableObject
{
    [SerializeField] private float _duration;
    [SerializeField] private Ease _ease;

    public void ScaleIn(GameObject gameObject,TweenCallback callback = null)
    {
        gameObject.transform.DOScale(Vector3.one, _duration).SetEase(_ease).OnComplete(()=>{callback?.Invoke();});
    }   

    public Tween ScaleOut(GameObject gameObject, TweenCallback callback = null)
    {
        return gameObject.transform.DOScale(Vector3.zero, _duration).SetEase(_ease).OnComplete(()=>{callback?.Invoke();});
    }
}
