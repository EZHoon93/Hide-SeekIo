using System.Collections;
using UnityEngine;
using System;
using Photon.Pun;

public class PlayerHealth : LivingEntity
{
    [SerializeField] ParticleSystem _recoveryEffect;
    PlayerStat _playerStat;
    public override int currHp
    {
        get => base.currHp;
        set
        {
            //회복 이라면
            if(currHp < value)
            {
                _recoveryEffect?.Play();
            }
            base.currHp = value;
            
        }
    }

    public override bool Dead { get => base.Dead;
        protected set
        {
            base.Dead = value;
            if (value)
            {
                _playerStat.animator.SetTrigger("Die");
            }
        }
    }

    public event Action onReviveEvent;


    
    protected void Awake()
    {
        _playerStat = GetComponent<PlayerStat>();
    }

    public override void OnPhotonInstantiate()
    {
        maxHp = _playerStat.maxHp;
        base.OnPhotonInstantiate();
        //AddRenderer(_playerCharacter.GetRenderController());
    }

    [PunRPC]
    public override void Die()
    {
        Managers.effectManager.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position, 0);
        base.Die();
    }

   
}
