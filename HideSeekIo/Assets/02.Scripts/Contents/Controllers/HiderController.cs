using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System;

public class HiderController : PlayerController, IPunObservable
{

    public override Define.Team GetTeam() => Define.Team.Hide;
    public override LivingEntity GetLivingEntity() => hiderHealth;



    public enum state
    {
        Idle,
        Attack,
        Die
    }

    public state _state;

    public state State
    {
        get => _state;
        set
        {
            if (_state == value) return;
            _state = value;
            switch (_state)
            {
                case state.Idle:

                    break;
                case state.Die:
                    //if (!photonView.IsMine)
                    //{
                    //    HandleDeath();
                    //}
                    break;
            }
        }
    }


    public HiderHealth hiderHealth { get; private set; }
    public HiderMove hiderMove { get; private set; }
    public HiderInput hiderInput{ get; private set; }

    protected override bool isDead { get => hiderHealth.Dead; }


    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
        if (stream.IsWriting)
        {
            stream.SendNext(_state);


        }
        else
        {
            State = (state)stream.ReceiveNext();

        }
    }

    protected virtual void Awake()
    {
        hiderHealth = GetComponent<HiderHealth>();
        hiderMove = GetComponent<HiderMove>();
        hiderInput = GetComponent<HiderInput>();

        hiderHealth.onDeath += HandleDeath;
        hiderHealth.Team = Define.Team.Hide;


        TimeCoinAmount = 1;

    }


    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        hiderHealth.OnPhotonInstantiate();
        hiderMove.OnPhotonInstantiate();
        hiderInput.OnPhotonInstantiate();
    }


    //public override void InitSetup()
    //{
    //    base.InitSetup();
    //    //humanHealth.enabled = true;
    //    //humanMove.enabled = true;
    //    //hiderInput.enabled = true;
    //    //GetComponent<CharacterController>().enabled = true;
    //    //humanMove.SetupMoveSpeed(DataManager.instance.EtcDataDic["Hms1"].values);    //이동속도 세팅
    //    //humanMove.MaxEnergy = DataManager.instance.EtcDataDic["Hen1"].values;



    //    //AI나 로컬유저가 아니라면 실행X
    //    if (!photonView.IsMine || this.gameObject.IsValidAI()) return;
    //}


    protected override void HandleDeath()
    {
        base.HandleDeath();
        State = state.Die;
        //humanMove.Stop();   //캐릭 멈춤
        //humanHealth.enabled = false;
        //humanMove.enabled = false;
        //baseInput.enabled = false;
        GetComponent<CharacterController>().enabled = false;

        //보이게 
        //var hideInFogs = GetComponentsInChildren<HideInFog>();
        //foreach (var h in hideInFogs)
        //{
        //    h.minFogStrength = 1.0f;
        //}
        //사망 UI
        GameManager.Instance.HumanDie(this.photonView.ViewID);

        //AI도 포함
        if (photonView.IsMine)
        {
            Util.CallBackFunction(this, 4.0f, () =>
            {
                PhotonNetwork.Destroy(this.gameObject);
                if (this.IsMyCharacter() == false) return;
                //PlayerInfo.EndGame(10, 10);
                //UIManager.instance.SetActive_Inventory(false, Define.Team.Human);
            });
        }

    }

   
}
