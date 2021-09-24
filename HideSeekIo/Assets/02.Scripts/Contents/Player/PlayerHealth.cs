using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
public class PlayerHealth : LivingEntity
{
    public event Action onReviveEvent;

    public override bool Dead { get => base.Dead;
        protected set
        {
            base.Dead = value;
            if (value)
            {
                _playerCharacter.animator.SetTrigger("Die");
            }
        }
    }
    PlayerCharacter _playerCharacter;

    private void Awake()
    {
        _playerCharacter = GetComponent<PlayerCharacter>();
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        AddRenderer(_playerCharacter.GetRenderController());
    }

    [PunRPC]
    public override void Die()
    {
        EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position, 0);
        base.Die();
        _playerCharacter.animator.SetTrigger("Die");
    }

    public void Revive()
    {
        onReviveEvent?.Invoke();
        //_animator.transform.localPosition = Vector3.zero;
        //_animator.gameObject.layer = (int)Define.Layer.Hider;
        EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position, 0);

    }

}
