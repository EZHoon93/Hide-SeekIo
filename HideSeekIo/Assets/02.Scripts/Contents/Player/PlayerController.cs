using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;
using System.Linq;
using FoW;
using System.Collections.Generic;
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
    public PlayerStat playerStat { get; set; }
    public Character_Base character_Base{ get; set; }

    public List<int> statTypeList = new List<int>();
    //public InGameItemController[] itemInventory { get; protected set; } = new InGameItemController[3];

    public event Action<PhotonView> changeOnwerShip;

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
    public void ChangeOwnerShip()
    {
        print("ChangeOwnerShip PlayerController    ");
        Managers.Game.myPlayer = this;
        playerInput.ChangeOwnerShip();
        playerMove.ChangeOwnerShip();
        playerShooter.ChangeOwnerShip();
        changeOnwerShip?.Invoke(this.photonView);

    }

    private void Awake()
    {
        playerHealth = this.gameObject.GetOrAddComponent<PlayerHealth>();
        playerInput = this.gameObject.GetOrAddComponent<PlayerInput>();
        playerShooter = this.gameObject.GetOrAddComponent<PlayerShooter>();
        playerMove = this.gameObject.GetOrAddComponent<PlayerMove>();
        playerStat = this.gameObject.GetOrAddComponent<PlayerStat>();
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

    //public void UPStatPointToServer(Define.StatType newStat)
    //{
    //    Hashtable prevHashtable = new Hashtable()
    //    {
    //        { "vID", this.ViewID() },
    //        { "oID" , 3 }
    //        //{ "st" , statTypeList.ToArray() },
    //    };
    //    //statTypeList.Add((int)newStat);
    //    Hashtable nextHashtable = new Hashtable()
    //    {
    //        { "vID", this.ViewID() },
    //        { "oID" , 4 }
    //        //{ "st" , statTypeList.ToArray() }
    //    };
    //    //PhotonGameManager.Instacne.SendEvent(Define.PhotonOnEventCode.AbilityCode, EventCaching.RemoveFromRoomCache, prevHashtable);
    //    //PhotonGameManager.Instacne.SendEvent(Define.PhotonOnEventCode.AbilityCode, EventCaching.AddToRoomCacheGlobal, nextHashtable);
    //}

    public void RecvieStatDataByServer(int[] dataArray)
    {
        
    }
}

