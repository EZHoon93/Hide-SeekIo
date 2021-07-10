using System.Collections;

using UnityEngine;
using Photon.Pun;
using System;

public class HiderHealth : LivingEntity
{
    //[SerializeField] HiderCage _cageObject;
    Animator _animator;

    public event Action onReviveEvent;

    protected override void Awake()
    {
        base.Awake();
        Team = Define.Team.Hide;
        //_cageObject.reviveEvent += Revive;
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        _animator = GetComponentInChildren<Animator>();
        //_cageObject.gameObject.SetActive(false);
    }

    [PunRPC]
    public override void Die()
    {
        base.Die();
        //_cageObject.gameObject.SetActive(true);
        //_animator.transform.localPosition = new Vector3(0, 0.3f, 0);
        _animator.SetTrigger("Die");
        _animator.gameObject.SetLayerRecursively((int)Define.Layer.Cage);
        EffectManager.Instance.EffectOnLocal(Define.EffectType.Death, this.transform.position, 0 );
        
        //this.gameObject.SetLayerRecursively((int)Define.Layer.Seeker);

    }

    public void Revive()
    {
        onReviveEvent?.Invoke();
        _animator.transform.localPosition = Vector3.zero;
        _animator.gameObject.layer = (int)Define.Layer.Hider;
        EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position, 0);

    }



}
