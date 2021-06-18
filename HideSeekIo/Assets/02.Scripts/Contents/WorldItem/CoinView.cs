using DG.Tweening;

using UnityEngine;

public sealed class CoinView : MonoBehaviour
{
    [SerializeField] private float _shakeScaleDuration = 1;
    [SerializeField] private float _hideScaleDuration = .25f;
    [SerializeField] public int Value = 1;

    public void Collect()
    {
        enabled = false;
        transform.DOShakeScale(_shakeScaleDuration);
        transform.DOScale(Vector3.zero, _hideScaleDuration).SetDelay(_shakeScaleDuration);
    }

    private void Update()
    {
        transform.Rotate(Vector3.one);
    }
}
