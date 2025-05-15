using System.Collections;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "SwapAnimationSO", menuName = "Scriptable Objects/DiamondAnimation/SwapAnimationSO")]
public class SwapAnimationSO : ScriptableObject
{
    [SerializeField] private float _duration;
    [SerializeField] private Ease _ease;

    public IEnumerator Swap(GameObject a, GameObject b, TweenCallback callback = null)
    {
        Vector3 posA = a.transform.position;
        Vector3 posB = b.transform.position;
        a.transform.DOMove(posB, _duration).SetEase(_ease);
        yield return b.transform.DOMove(posA, _duration).SetEase(_ease).OnComplete(()=>{
            callback?.Invoke();
        }).WaitForCompletion();
    }
}