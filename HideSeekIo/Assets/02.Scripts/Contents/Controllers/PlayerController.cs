using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;
using System.Linq;
using FoW;
using System.Collections.Generic;
using HasTable = ExitGames.Client.Photon.Hashtable;
//[RequireComponent(typeof(PlayerInput))]
//[RequireComponent(typeof(PlayerHealth))]
//[RequireComponent(typeof(PlayerShooter))]
//[RequireComponent(typeof(PlayerMove))]

public class PlayerController : MonoBehaviourPun
{
    bool _isGrass;
    bool _isDetectedInGrass;
    bool _isTransSkill;
    bool _isWarining;
    
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

    public FogOfWarController fogOfWarController => playerHealth.fogController;


    //public bool isGrass
    //{
    //    get => _isGrass;
    //    set
    //    {
            
    //        if (_isGrass == value) return;
    //        fogOfWarController.hideInFog.isGrass = value;
    //        _isGrass = value;
    //        if (this.IsMyCharacter())
    //        {
    //        }
    //        else
    //        {
    //        }
    //    }
    //}

    //public bool isGrass
    //{
    //    get => _isGrass;
    //    set
    //    {

    //        if (_isGrass == value) return;
    //        fogOfWarController.hideInFog.isGrass = value;
    //        _isGrass = value;
    //        if (this.IsMyCharacter())
    //        {
    //        }
    //        else
    //        {
    //        }
    //    }
    //}

    //public bool isGrass
    //{
    //    get => _isGrass;
    //    set
    //    {

    //        if (_isGrass == value) return;
    //        fogOfWarController.hideInFog.isGrass = value;
    //        _isGrass = value;
    //        if (this.IsMyCharacter())
    //        {
    //        }
    //        else
    //        {
    //        }
    //    }
    //}


    public List<int> statTypeList = new List<int>();
    public event Action<PhotonView> changeOnwerShip;


    private void Awake()
    {
        playerHealth = this.gameObject.GetOrAddComponent<PlayerHealth>();
        playerHealth.onDeath += HandleDeath;
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
        _isWarining = false;
    }

    public virtual void OnPhotonInstantiate(PhotonView photonView)
    {
        playerInput.OnPhotonInstantiate();
        playerHealth.OnPhotonInstantiate();
        playerMove.OnPhotonInstantiate();
        playerShooter.OnPhotonInstantiate();
        playerCharacter.OnPhotonInstantiate(this);
        playerStat.OnPhotonInstantiate();
        _playerUI.OnPhotonInstantiate();
        fogOfWarController.OnPhotonInstantiate(playerHealth);
        fogOfWarController.transform.ResetTransform(playerCharacter.characterAvater.transform);
        statTypeList.Clear();
        ChangeTeam(Define.Team.Hide);
    }

    public virtual void OnPreNetDestroy(PhotonView rootView)
    {

    }

    public void ChangeOwnerShip()
    {
        playerInput.ChangeOwnerShip();
        playerMove.ChangeOwnerShip();
        playerShooter.ChangeOwnerShip();
        playerCharacter.ChangeOnwerShip(this);
        _playerUI.ChangeOwnerShip();

        changeOnwerShip?.Invoke(this.photonView);
        if (this.IsMyCharacter())
        {
            Managers.Game.myPlayer = this;
        }
    }


    public void ChangeTeam(Define.Team team)
    {
        switch (team)
        {
            case Define.Team.Hide:
                this.gameObject.layer = (int)Define.Layer.Hider;
                fogOfWarController.fogOfWarUnit.circleRadius = 7;
                fogOfWarController.fogOfWarUnit.shapeType = FogOfWarShapeType.Circle;
                fogOfWarController.fogOfWarUnit.offset = Vector3.zero;

                break;
            case Define.Team.Seek:
                this.gameObject.layer = (int)Define.Layer.Seeker;
                fogOfWarController.fogOfWarUnit.circleRadius =7;
                fogOfWarController.fogOfWarUnit.shapeType = FogOfWarShapeType.Circle;
                fogOfWarController.fogOfWarUnit.offset = new Vector2(0, 0);
                //fogOfWarController.fogOfWarUnit.circleRadius = 16;
                //fogOfWarController.fogOfWarUnit.shapeType = FogOfWarShapeType.Circle;
                //fogOfWarController.fogOfWarUnit.offset = Vector3.zero;
                //fogOfWarController.fogOfWarUnit.cellBased = true;
                break;
        }

        playerHealth.Team = team;
        playerStat.ChangeTeam(team);
        playerInput.ChangeTeam(team);
        if (this.IsMyCharacter())
        {
            CameraManager.Instance.ChangeTeam(team);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var enterTrigger = other.gameObject.GetComponent<ICanEnterTriggerPlayer>();
        if (enterTrigger != null)
        {
            enterTrigger.Enter(this , other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var exitTrigger = other.gameObject.GetComponent<ICanExitTriggerPlayer>();
        if (exitTrigger != null)
        {
            exitTrigger.Exit(this , other);
        }
    }
}

