using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
public class PlayerHealth : LivingEntity
{
    public Character_Base character_Base { get; set; }
    public event Action onReviveEvent;
   
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        character_Base = GetComponent<Character_Base>();
        //_animator = GetComponentInChildren<Animator>();
    }

    [PunRPC]
    public override void Die()
    {
        EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position, 0);
        base.Die();
        //_animator.SetTrigger("Die");
    }

    public void Revive()
    {
        onReviveEvent?.Invoke();
        //_animator.transform.localPosition = Vector3.zero;
        //_animator.gameObject.layer = (int)Define.Layer.Hider;
        EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position, 0);

    }

}
