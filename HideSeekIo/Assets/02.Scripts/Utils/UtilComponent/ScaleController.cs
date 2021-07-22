using System.Collections;

using UnityEngine;
using DG.Tweening;

public class ScaleController : Poolable
{
    [SerializeField] Vector3 _destSize;
    [SerializeField] float _time;

    private void OnEnable()
    {
        this.transform.localScale = Vector3.zero;
        this.transform.DOScale(_destSize, _time).OnComplete(Push); ;
    }

    public override void Push()
    {
        this.gameObject.SetActive(false);
    }
}
