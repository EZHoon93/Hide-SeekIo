using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EffectObject : Poolable
{
    [SerializeField] float _time = 2;
    [SerializeField] ParticleSystem _effectParticle;

    private void Reset()
    {
        _effectParticle = this.transform.GetChild(0).GetComponent<ParticleSystem>();
    }
    private void OnEnable()
    {
        _effectParticle.Play();
        CancelInvoke("AfaterDestroy");
        Invoke("AfaterDestroy", _time);
    }
    void AfaterDestroy()
    {
        Managers.Pool.Push(this);
    }
}