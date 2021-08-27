using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;
using System.Linq;
using FoW;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
//[RequireComponent(typeof(PlayerInput))]
//[RequireComponent(typeof(PlayerHealth))]
//[RequireComponent(typeof(PlayerShooter))]
//[RequireComponent(typeof(PlayerMove))]

public class PlayerController : MonoBehaviourPun
{
    public string NickName { get;  set; }
    public Define.Team Team => Define.Team.Hide;
    public PlayerInput playerInput{ get; private set; }
    public PlayerHealth playerHealth{ get; private set; }
    public PlayerShooter playerShooter{ get; private set; }
    public PlayerMove playerMove{ get; private set; }
    public Character_Base character_Base{ get; set; }

    public List<int> statTypeList = new List<int>();
    //public InGameItemController[] itemInventory { get; protected set; } = new InGameItemController[3];


    public virtual void OnPhotonInstantiate(PhotonView photonView)
    {

        playerInput.OnPhotonInstantiate();
        playerHealth.OnPhotonInstantiate();
        playerMove.OnPhotonInstantiate();
        playerShooter.OnPhotonInstantiate();
        character_Base.OnPhotonInstantiate();
        statTypeList.Clear();
        ChangeTeam(Define.Team.Hide);
    }

    private void Awake()
    {
        playerHealth = this.gameObject.GetOrAddComponent<PlayerHealth>();
        playerInput = this.gameObject.GetOrAddComponent<PlayerInput>();
        playerShooter = this.gameObject.GetOrAddComponent<PlayerShooter>();
        playerMove = this.gameObject.GetOrAddComponent<PlayerMove>();
    }

    protected virtual void HandleDeath()
    {

    }

    public void ChangeTeam(Define.Team team)
    {
        switch (team)
        {
            case Define.Team.Hide:
                this.gameObject.layer = (int)Define.Layer.Hider;
                break;
            case Define.Team.Seek:
                this.gameObject.layer = (int)Define.Layer.Seeker;
                Managers.Spawn.WeaponSpawn(Define.Weapon.Melee2, this.playerShooter);
                break;
        }
    }


    ////?????? ?????? ???? ?????? ???? ????
    //protected virtual void FixedUpdate()
    //{
    //    n_sync = false;
    //}

    ////???????? ???????? true => 
    //[PunRPC]
    //public void CallSync()
    //{
    //    n_sync = true;
    //}

    private void OnTriggerEnter(Collider other)
    {
        var enterTrigger = other.gameObject.GetComponent<IEnterTrigger>();
        if (enterTrigger != null)
        {
            enterTrigger.Enter(this.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var exitTrigger = other.gameObject.GetComponent<IExitTrigger>();
        if (exitTrigger != null)
        {
            exitTrigger.Exit(this.gameObject);
        }
    }

    public void UPStatPointToServer(Define.StatType newStat)
    {
        Hashtable prevHashtable = new Hashtable()
        {
            { "vID", this.ViewID() },
            { "st" , statTypeList.ToArray() }
        };
        statTypeList.Add((int)newStat);
        Hashtable nextHashtable = new Hashtable()
        {
            { "vID", this.ViewID() },
            { "st" , statTypeList.ToArray() }
        };
        PhotonGameManager.Instacne.SendEvent(Define.PhotonOnEventCode.AbilityCode, EventCaching.RemoveFromRoomCache, prevHashtable);
        PhotonGameManager.Instacne.SendEvent(Define.PhotonOnEventCode.AbilityCode, EventCaching.AddToRoomCacheGlobal, nextHashtable);

        //int viewId = playerController.photonView.ViewID;
        //byte keyCode = (byte)Define.EventCode.AbilityCode;
        //bool isAI = false;
        //List<int> sendData = new List<int>();   //보낼데이터
        //foreach (var v in playerController._buyAbilityList)
        //    sendData.Add((int)v);   //현재 데이터들을 갖고옴
        //sendData.Add((int)abilityType);  //새로 추가 데이터
        ////포톤으로 보낼 데이터 만든다
        //Hashtable HT = new Hashtable();
        //HT.Add("Pv", viewId);   //적용할 캐릭 뷰 아이디
        //RemoveEvent(keyCode, HT);   //현재까지의 키코드로 데이터제거 보냄
        //HT.Add("Ab", sendData.ToArray());       //int[] 형식
        //SendEvent(viewId, keyCode, isAI, HT);   //데이터 보내기
    }

}

