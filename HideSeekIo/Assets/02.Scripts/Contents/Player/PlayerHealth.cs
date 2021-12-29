using System.Collections;
using UnityEngine;
using System;
using Photon.Pun;

public class PlayerHealth : LivingEntity
{
    public event Action onReviveEvent;
    [SerializeField] ParticleSystem _recoveryEffect;
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
                _playerCharacter.animator.SetTrigger("Die");
            }
        }
    }
    PlayerCharacter _playerCharacter;

    float _recoveryLastTime;
    float _recoveryTimeBet = 1.0f;

    private void Awake()
    {
        _playerCharacter = GetComponent<PlayerCharacter>();
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        _recoveryLastTime = 0;
        AddRenderer(_playerCharacter.GetRenderController());
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        //로컬 만 실행
        if (currHp >= maxHp || photonView.IsMine == false) return;
        if (Time.time >= _recoveryLastTime + _recoveryTimeBet)
        {
            _recoveryLastTime = Time.time;
            currHp = Mathf.Clamp(currHp + (int)(maxHp * 0.2f), 0, maxHp);
        }
    }
 

  

    [PunRPC]
    public override void OnDamage(int damagerViewId, int damage, Vector3 hitPoint)
    {
        base.OnDamage(damagerViewId, damage, hitPoint);
        _recoveryLastTime = Time.time + _recoveryTimeBet * 5;   //3배정도뒤
    }

    [PunRPC]
    public override void Die()
    {
        Managers.effectManager.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position, 0);
        base.Die();
    }

   
}
