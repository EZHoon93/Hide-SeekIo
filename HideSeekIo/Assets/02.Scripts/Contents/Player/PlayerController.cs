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

public class PlayerController : MonoBehaviourPun , IGrassDetected
{
    bool _isGrass;
    
    [SerializeField] PlayerUI _playerUI;
    [SerializeField] PlayerGrassDetect _playerGrassDetect;

    public string NickName { get; set; }
    public Define.Team Team => playerHealth.Team;
    public PlayerInput playerInput { get; private set; }
    public PlayerHealth playerHealth { get; private set; }
    public PlayerShooter playerShooter { get; private set; }
    public PlayerMove playerMove { get; private set; }
    public PlayerStat playerStat { get; set; }
    public PlayerCharacter playerCharacter { get; set; }
    public PlayerUI playerUI => _playerUI;

    FogOfWarController _fogOfWarController => playerHealth.fogController;
    public bool isGrass
    {
        get => _isGrass;
        set
        {
            
            if (_isGrass == value) return;
            _fogOfWarController.hideInFog.isGrass = value;
            _isGrass = value;
            if (this.IsMyCharacter())
            {

            }
            else
            {
            }
        }
    }


    public List<int> statTypeList = new List<int>();
    //public InGameItemController[] itemInventory { get; protected set; } = new InGameItemController[3];

    public event Action<PhotonView> changeOnwerShip;


    private void Awake()
    {
        playerHealth = this.gameObject.GetOrAddComponent<PlayerHealth>();
        playerInput = this.gameObject.GetOrAddComponent<PlayerInput>();
        playerShooter = this.gameObject.GetOrAddComponent<PlayerShooter>();
        playerMove = this.gameObject.GetOrAddComponent<PlayerMove>();
        playerStat = this.gameObject.GetOrAddComponent<PlayerStat>();
        playerCharacter = this.gameObject.GetOrAddComponent<PlayerCharacter>();
        _playerGrassDetect.gameObject.SetActive(false);
        _playerUI.SetupPlayer(this);
    }

    protected virtual void HandleDeath()
    {
        playerHealth.enabled = false;
        playerShooter.enabled = false;
        playerMove.enabled = false;
        playerStat.enabled = false;
        playerCharacter.enabled = false;
        playerInput.enabled = false;
    }

    public virtual void OnPhotonInstantiate(PhotonView photonView)
    {

        playerInput.OnPhotonInstantiate();
        playerHealth.OnPhotonInstantiate();
        playerMove.OnPhotonInstantiate();
        playerShooter.OnPhotonInstantiate();
        _playerUI.OnPhotonInstantiate();
        statTypeList.Clear();
        isGrass = false;
        //playerHealth.AddRenderer(playerCharacter.character_Base.characterAvater.renderController);
        ChangeTeam(Define.Team.Hide);
    }
    public void ChangeOwnerShip()
    {
        playerInput.ChangeOwnerShip();
        playerMove.ChangeOwnerShip();
        playerShooter.ChangeOwnerShip();
        changeOnwerShip?.Invoke(this.photonView);
        if (this.IsMyCharacter())
        {
            _playerGrassDetect.gameObject.SetActive(true);
            Managers.Game.myPlayer = this;
        }
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
                break;
        }
        playerHealth.Team = team;
        //playerStat.Recive_ChangeTeam();
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

    private void LateUpdate()
    {
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

    public void ChangeTransParent(bool active)
    {
        _fogOfWarController.hideInFog.isGrassDetected = active;
    }
}

