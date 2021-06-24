using System.Collections;

using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class MoveBase : MonoBehaviourPun, IPunObservable
{
    InputBase _inputBase;
    AttackBase _attackBase;
    [SerializeField] Transform _modelTransform;  //회전시킬 객체,

    private Vector2 m_moveInput;


    List<float> _moveBuffRatioList = new List<float>(); //캐릭에 슬로우및이속증가 버퍼리스트
    protected float _totRatio;    //버퍼리스트 합계산한 최종 이속 증/감소율
    
    public float MoveSpeed { get; protected set; }
    public int RotationSpeed { get; protected set; } = 15;
    public float ResultSpeed { get; protected set; }


    protected virtual void Awake()
    {
        _inputBase = GetComponent<InputBase>();
        _attackBase = GetComponent<AttackBase>();
    }
    public virtual void OnPhotonInstantiate()
    {

    }
    protected virtual void Update()
    {
        if (!this.photonView.IsMine)
        {
            if(m_moveInput.sqrMagnitude == 0)
            {
                return;
            }

            var newRotation = UtillGame.GetWorldRotation_ByInputVector(m_moveInput);
            this.transform.rotation = Quaternion.Slerp(this.transform.localRotation, newRotation, RotationSpeed * Time.deltaTime);
            
        }
    }

  

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
            stream.SendNext(_inputBase.MoveVector);
        }
        else
        {
            var  n_moveInput = (Vector2)stream.ReceiveNext();
            //if (n_moveInput.sqrMagnitude == 0) return;
            m_moveInput = n_moveInput;
            
        }
    }

}
