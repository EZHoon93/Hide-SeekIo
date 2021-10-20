using UnityEngine;
using Photon.Pun;
using System;
using FoW;
using System.Collections.Generic;

public class PlayerController : MonoBehaviourPun
{
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
    public PlayerGrassDetect playerGrassDetect => _playerGrassDetect;

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
        playerStat.OnPhotonInstantiate();   //Stat 이 제일먼저!!
        _playerUI.OnPhotonInstantiate();   
        playerInput.OnPhotonInstantiate();
        playerHealth.OnPhotonInstantiate();
        playerMove.OnPhotonInstantiate();
        playerShooter.OnPhotonInstantiate();
        playerCharacter.OnPhotonInstantiate(this);
        _playerGrassDetect.gameObject.SetActive(false);
        fogOfWarController.OnPhotonInstantiate(playerHealth);
        fogOfWarController.transform.ResetTransform(playerCharacter.character_Base.transform);
        statTypeList.Clear();
        ChangeTeam(Define.Team.Hide);
    }
    public void ChangeAI()
    {
        playerInput.ChangeAI();
        this.gameObject.tag = "AI";
    }

    public virtual void OnPreNetDestroy(PhotonView rootView)
    {

    }


    /// <summary>
    /// 유저가 소유권을 가져왔을때 실행
    /// </summary>
    public void ChangeOwnerShipOnUser()
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
            _playerGrassDetect.gameObject.SetActive(true);

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
                fogOfWarController.fogOfWarUnit.circleRadius =3.5f;
                fogOfWarController.fogOfWarUnit.shapeType = FogOfWarShapeType.Circle;
                fogOfWarController.fogOfWarUnit.offset = new Vector2(0, 0);
                //fogOfWarController.fogOfWarUnit.circleRadius = 16;
                //fogOfWarController.fogOfWarUnit.shapeType = FogOfWarShapeType.Circle;
                //fogOfWarController.fogOfWarUnit.offset = Vector3.zero;
                //fogOfWarController.fogOfWarUnit.cellBased = true;
                if (PhotonNetwork.IsMasterClient)
                {
                    Managers.Spawn.WeaponSpawn(Define.Weapon.Hammer, this);
                }
                break;
        }

        playerHealth.Team = team;
        playerStat.ChangeTeam(team);
        playerInput.ChangeTeam(team);
        playerUI.ChangeTeam(team);
        if (this.IsMyCharacter())
        {
            CameraManager.Instance.ChangeTeamByTargetView(this);
        }
    }
   
    void Revive()
    {

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

    private void OnCollisionStay(Collision collision)
    {
         collision.collider.CompareTag("Grass");
    }
}

