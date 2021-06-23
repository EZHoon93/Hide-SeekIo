using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EffectObject : Poolable
{
    [SerializeField] float _time = 2;
    private void OnEnable()
    {
        CancelInvoke("AfaterDestroy");
        Invoke("AfaterDestroy", _time);
    }
    void AfaterDestroy()
    {
        Managers.Pool.Push(this);
    }
}