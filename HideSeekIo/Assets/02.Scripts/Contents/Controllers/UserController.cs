using Photon.Pun;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;
public class UserController : MonoBehaviourPun, IPunInstantiateMagicCallback ,IOnPhotonViewPreNetDestroy,IPunObservable
{
    #region PlayerController
    PlayerController _playerController;

    public PlayerController playerController
    {
        get => _playerController;
        set
        {
            var prevPlayer = _playerController;
            _playerController = value;
            if (photonView.IsMine)
            {
                if(_playerController != null)
                {
                    //Managers.cameraManager.cameraState = Define.CameraState.MyPlayer;
                    //Managers.cameraManager.SetupTargetPlayerController(_playerController);
                    Managers.Game.NotifyGameEvent(Define.GameEvent.MyPlayerActive, true);
                }
                // null 이라면..
                else
                {
                    //prevPlayer.playerInput.SetActiveUserControllerJoystick(false);
                    //Managers.cameraManager.FindNextPlayer();
                    if (prevPlayer)
                    {
                        prevPlayer.playerInput.SetActiveUserControllerJoystick(false);
                    }
                    Managers.Game.NotifyGameEvent(Define.GameEvent.MyPlayerActive, false);
                }
            }
        }
    }
    #endregion
    public string userNickName;
    public int userNumber;

    public bool IsJoin => (bool)PhotonNetwork.LocalPlayer.CustomProperties["jn"];

    #region Interface

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
  
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var infoData = info.photonView.InstantiationData;
        if (infoData == null)
        {
            return;
        }
        info.Sender.TagObject = this;
        userNumber = (int)infoData[0];
        userNickName = (string)infoData[1]; //로컬넘버


        Managers.Game.RegisterUserController(info.Sender.ActorNumber, this);

        if (photonView.IsMine)
        {
            Managers.Game.myUserController = this;
            //Managers.Game.AddListenrOnGameEvent(Define.GameEvent.GameJoin, GameJoinCallBack);
        }
    }

    /// <summary>
    /// 방장은
    /// </summary>
    public void OnPreNetDestroy(PhotonView rootView)
    {
        Managers.Game.UnRegisterLivingEntity(rootView.ControllerActorNr);
        if (PhotonNetwork.IsMasterClient && playerController)
        {
            playerController.playerInput.ChangePlayerType(Define.PlayerType.AI);
        }
    }

    #endregion

    /// <summary>
    /// 서버로 게임참여 전송
    /// </summary>
    public void GameEnterToServer()
    {
        if (IsJoin)
        {
            return;
        }
        //비참여인상태에서만 보낼수있음..
        var currentSkinInfo = PlayerInfo.userData.GetCurrentAvater();
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {
        { "jn", true },
        { "ch" ,0},   // 캐릭아바타스킨
        { "we" ,0},   //무기아바타스킨
        { "ac" , -1},   //악세사리스킨

        });
    }

    /// <summary>
    /// 서버로 게임 나가기 전송
    /// </summary>
    public void GameExitToServer()
    {
        if (!IsJoin)
        {
            return;
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {
            { "jn", false },
        });
    }


    /// <summary>
    /// 포톤으로부터 게임참여 여부 콜백
    /// </summary>
    public void GameJoinCallBackByPhotonServer(bool isJoin)
    {
        if(photonView.IsMine == false)
        {
            return;
        }
        //참여라면
        if (isJoin)
        {
            Managers.cameraManager.cameraState = Define.CameraState.Observer;
        }
        //나간다면
        else
        {
            Managers.cameraManager.cameraState = Define.CameraState.Auto;

            if (playerController)
            {
                photonView.RPC("BufferedMyPlayerToAI", RpcTarget.AllBufferedViaServer);
            }   
        }
        Managers.Game.NotifyGameEvent(Define.GameEvent.GameJoin, isJoin);
        //Managers.cameraManager.observerController.SetActive(IsJoin);
    }

    /// <summary>
    /// 내캐릭 AI로 넘김..
    /// </summary>
    [PunRPC]
    public void BufferedMyPlayerToAI()
    {
        if(playerController == null)
        {
            return;
        }
        this.photonView.ControllerActorNr = 0;
        playerController.gameObject.tag = "AI";
        playerController.playerInput.ChangePlayerType(Define.PlayerType.AI);
        playerController = null;
    }
}


