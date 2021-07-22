
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;

public class Timer_Glue : TimerItem
{
    TimerItemController _timerItemController;
    List<MoveBase> _moveBase = new List<MoveBase>();
    [SerializeField] ParticleSystem _effectAcid;

    bool _isSizeChange;

    private void Awake()
    {
        _timerItemController = GetComponent<TimerItemController>();
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var sendTime =  info.SentServerTime;
        if(PhotonNetwork.Time - info.SentServerTime < 1)
        {
            EffectManager.Instance.EffectOnLocal(Define.EffectType.AcidExp, this.transform.position + new Vector3(0,0.5f,0), 0);
        }
        _moveBase.Clear();
        _effectAcid.Play();
        _isSizeChange = false;
        this.transform.localScale = new Vector3(2.5f, 1, 2.5f);
    }

    public override void EndTime()
    {
       foreach(var m in _moveBase)
        {
            m.AddMoveBuffList(-0.5f, false);
        }
    }

    private void Update()
    {
        if( _timerItemController.RemainTime < 0.5f && _isSizeChange == false)
        {
            _isSizeChange = true;
            this.transform.DOScale(Vector3.zero, 0.5f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        var moveBase = other.GetComponent<MoveBase>();
        if (moveBase != null)
        {
            _moveBase.Add(moveBase);
            moveBase.AddMoveBuffList(-0.5f, true);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        print(other.gameObject.name + "> 나감");

        var moveBase = other.GetComponent<MoveBase>();
        if (moveBase != null)
        {
            _moveBase.Add(moveBase);
            moveBase.AddMoveBuffList(-0.5f, false);
        }
    }
}
