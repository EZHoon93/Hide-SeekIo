using UnityEngine;
using Photon.Pun;
using System;
using FoW;
using Photon.Realtime;

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

    //public PlayerCharacter playerCharacter { get; set; }

    public PlayerUI playerUI => _playerUI;
    public FogOfWarController fogOfWarController => playerHealth.fogController;
    public PlayerGrassDetect playerGrassDetect => _playerGrassDetect;
    public Animator animator => playerStat.animator;
    public CharacterAvater characterAvater => playerStat.characterAvater;

    public event Action<PhotonView> changeOnwerShip;
    public event Action<PhotonView> onPhotonDestroyEvent;
    public event Action<PlayerController> onPhotonInstantiateEvent;


    private void Awake()
    {
        playerHealth = this.gameObject.GetOrAddComponent<PlayerHealth>();
        playerHealth.onDeath += HandleDeath;
        playerInput = this.gameObject.GetOrAddComponent<PlayerInput>();
        playerShooter = this.gameObject.GetOrAddComponent<PlayerShooter>();
        playerMove = this.gameObject.GetOrAddComponent<PlayerMove>();
        playerStat = this.gameObject.GetOrAddComponent<PlayerStat>();
        _playerUI.SetupPlayer(this);
    }

  

    #region Call Back Event
    public virtual void OnPhotonInstantiate(PhotonView photonView)
    {
        //playerStat.OnPhotonInstantiate();   //Stat ?? ????????!!
        _playerUI.OnPhotonInstantiate();   
        playerInput.OnPhotonInstantiate();
        playerHealth.OnPhotonInstantiate();
        playerMove.OnPhotonInstantiate(this);
        playerShooter.OnPhotonInstantiate();
        //playerCharacter.OnPhotonInstantiate(this);
        _playerGrassDetect.gameObject.SetActive(false);
        //_playerObjectController.OnPhotonInstantiate(this);
        //_moveEnergyController.OnPhotonInstantiate(this);
        fogOfWarController.OnPhotonInstantiate();
        fogOfWarController.transform.ResetTransform(this.transform);
        fogOfWarController.fogOfWarUnit.circleRadius = 7;
        fogOfWarController.fogOfWarUnit.shapeType = FogOfWarShapeType.Circle;
        fogOfWarController.fogOfWarUnit.offset = Vector3.zero;
    }

    public virtual void OnPreNetDestroy(PhotonView rootView)
    {
        _playerUI.OnPreNetDestroy(rootView);
        playerInput.OnPreNetDestroy(rootView);
        playerShooter.OnPreNetDestroy(rootView);
        playerHealth.OnPreNetDestroy(rootView);
        //playerCharacter.OnPreNetDestroy(rootView);
        if (this.IsMyCharacter())
        {
            var myUserController = Managers.Game.myUserController;
            if (myUserController)
            {
                myUserController.playerController = null;
            }
            Managers.CameraManager.cameraState = Define.CameraState.Auto;

        }
    }
    #endregion

    public void ChangeTeam(Define.Team team)
    {
        switch (team)
        {
            case Define.Team.Hide:
                this.gameObject.layer = (int)Define.Layer.Hider;
                fogOfWarController.fogOfWarUnit.circleRadius = 7;
                fogOfWarController.fogOfWarUnit.shapeType = FogOfWarShapeType.Circle;
                fogOfWarController.fogOfWarUnit.offset = Vector3.zero;
                if (photonView.IsMine)
                {
                    //if(Managers.Game.gameStateController.gameStateType == Define.GameState.GameReady ||
                    //    Managers.Game.gameStateController.gameStateType == Define.GameState.Gameing)
                    //{
                    //    Managers.StatSelectManager.SetupRandomWeapon(this);
                    //    //Managers.Spawn.WeaponSpawn(Define.Weapon.Flash, this);
                    //} 
                }
                break;
            case Define.Team.Seek:
                this.gameObject.layer = (int)Define.Layer.Seeker;
                fogOfWarController.fogOfWarUnit.circleRadius = 3.5f;
                fogOfWarController.fogOfWarUnit.shapeType = FogOfWarShapeType.Circle;
                fogOfWarController.fogOfWarUnit.offset = new Vector2(0, 0);
                //fogOfWarController.fogOfWarUnit.circleRadius = 16;
                //fogOfWarController.fogOfWarUnit.shapeType = FogOfWarShapeType.Circle;
                //fogOfWarController.fogOfWarUnit.offset = Vector3.zero;
                //fogOfWarController.fogOfWarUnit.cellBased = true;
                if (photonView.IsMine)
                {
                    //if (Managers.Game.gameStateController.gameStateType == Define.GameState.GameReady ||
                    //    Managers.Game.gameStateController.gameStateType == Define.GameState.Gameing)
                    //{
                    //    Managers.Spawn.WeaponSpawn(Define.Weapon.Hammer, this);
                    //}
                }
                break;
        }

        playerHealth.Team = team;
        //playerStat.ChangeTeam(team);
        //playerInput.ChangeTeam(team);
        //playerUI.ChangeTeam(team);
        if (this.IsMyCharacter())
        {
            Managers.CameraManager.ChangeTeamByTargetView(this);
        }
    }

 


    protected virtual void SetActiveEnable(bool active)
    {
        playerHealth.enabled = false;
        playerShooter.enabled = false;
        playerMove.enabled = false;
        playerStat.enabled = false;
        //playerCharacter.enabled = false;
        playerInput.enabled = false;
    }

    void HandleDeath()
    {
        SetActiveEnable(false);

        Invoke("AfterDestory", 3.0f);
    }
    void AfterDestory()
    {
        Managers.Resource.PunDestroy(this);
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


    #region 






    #endregion
    #region



    ////???????????? ?????????!!
    //public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    //{
    //    //????????? ??????????????? ??????
    //    if (targetView.ViewID != this.ViewID()) return;

    //    //????????? ???????????? ?????? =>???????????????????????? 
    //    if (previousOwner.IsMasterClient)
    //    {
    //        //??????????????????  ???????????? ?????????????????????
    //        if (targetView.ControllerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
    //        {
    //            Managers.Game.myPlayer = this;
    //            _playerGrassDetect.gameObject.SetActive(true);

    //        }
    //    }
    //    //????????? ????????? ???????????? ?????? => AI??? ??????
    //    else
    //    {
    //        previousOwner.TagObject = null; //?????????????????? ??????.
    //        SetupAIPlayer();
    //    }

    //}



    //public void ChangeOwnerShipOnUser(bool isMyCharacter)
    //{
    //    //playerHealth.ChangeOwnerShipOnUser(isMyCharacter);
    //    //playerMove.ChangeOwnerShip();
    //    //playerShooter.ChangeOwnerShip();
    //    //playerCharacter.ChangeOnwerShip(this);
    //    //_playerObjectController.ChangeOwnerShipOnUser(isMyCharacter);
    //    //_moveEnergyController.ChangeOwnerShipOnUser(isMyCharacter);
    //    //_playerUI.ChangeOwnerShip(isMyCharacter);
    //    //changeOnwerShip?.Invoke(this.photonView);
    //    if (photonView.IsMine)
    //    {
    //        //Managers.buffManager.OnApplyBuffOnLocal(playerHealth, Define.BuffType.B_Speed, 15);

    //    }
    //    if (this.IsMyCharacter())
    //    {
    //        //Managers.Game.myPlayer= this;
    //    }
    //}





    //void SetupAIPlayer()
    //{
    //    ChangePlayerType(Define.PlayerType.AI);
    //}


    //public void ChangePlayerType(Define.PlayerType playerType)
    //{
    //    this.gameObject.tag = playerType.ToString();
    //    playerInput.ChangePlayerType(playerType);
    //}

    #endregion
}

