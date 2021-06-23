using System.Collections;

using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class MoveBase : MonoBehaviourPun, IPunObservable
{
    List<float> _moveBuffRatioList = new List<float>(); //캐릭에 슬로우및이속증가 버퍼리스트
    protected float _totRatio;    //버퍼리스트 합계산한 최종 이속 증/감소율
    protected float _animationValue;
    protected Animator _animator;
    //public enum MoveHearState
    //{
    //    Effect,
    //    NoEffect
    //}
    //protected MoveHearState n_moveHearState;

    //public MoveHearState HearState { get => n_moveHearState;  set { n_moveHearState = value; } }

    public float MoveSpeed { get; protected set; }

    public int RotationSpeed { get; protected set; } = 15;

    public float ResultSpeed { get; protected set; }

    public virtual void OnPhotonInstantiate()
    {
        _animator = GetComponentInChildren<Animator>();
    }
    public void SetupMoveSpeed(float initMoveSpeed) => MoveSpeed = initMoveSpeed;   //이동속도 세팅

    public void AddMoveBuffList(float ratio, bool isAdd)
    {
        if (isAdd)
        {

            _moveBuffRatioList.Add(ratio);
        }
        else
        {
            _moveBuffRatioList.Remove(ratio);
        }

        _totRatio = 0; ;
        foreach (var v in _moveBuffRatioList)
        {
            _totRatio += v;
        }

    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_animationValue);
        }
        else
        {
            _animationValue = (float)stream.ReceiveNext();
            _animator.SetFloat("Speed", _animationValue);
        }
    }

}
