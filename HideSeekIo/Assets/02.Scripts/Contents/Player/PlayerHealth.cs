using System.Collections;
using System.Collections.Generic;
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
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        _recoveryLastTime = 0;
        AddRenderer(_playerCharacter.GetRenderController());
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
        EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position, 0);
        base.Die();
        _playerCharacter.animator.SetTrigger("Die");
        if(Team == Define.Team.Hide)
        {
            Managers.Resource.PunDestroy(this);
        }
        else
        {
            StartCoroutine(Revive());
        }
    }

    IEnumerator Revive()
    {
        int remainTime = 5;
        while(remainTime >= 0)
        {
            if (this.IsMyCharacter())
            {
                var uiMain = Managers.UI.SceneUI as UI_Main;
                uiMain.UpdateNoticeText($"{remainTime} 초 후 부활 합니다");
            }
            remainTime--;
            yield return new WaitForSeconds(1.0f);
        }
        _playerCharacter.animator.SetTrigger("Die");

        onReviveEvent?.Invoke();    //부활 이벤트발생
    }

}
