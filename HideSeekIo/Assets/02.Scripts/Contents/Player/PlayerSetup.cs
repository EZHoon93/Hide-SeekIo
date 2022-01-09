
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
/// <summary>
/// 생서 및 파괴 시 
/// </summary>
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PhotonView))]
public class PlayerSetup : MonoBehaviourPun,IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy 
{
    [SerializeField] PhotonView _subPhotonView;
   

    #region CallBack Interface
    /// <summary>
    ///["nu"] = player.ActorNumber 넘버  ["nn"] = player.NickName 닉네임
    //["te"] = Define.Team.Hide  팀      ["ch"] = HT["ch"],  캐릭스킨
    //["we"] = HT["we"]//무기스킨          ["ac"] = HT["ac"], 악세스킨
    // 우선 AI로 생성.. 자기자신캐릭터는 버퍼를 보냄
    /// </summary>
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var infoData = info.photonView.InstantiationData;
        if (infoData == null) return;   //데이터가없으면 null
        //-데이터받아옴//
        var actorNumber = (int)infoData[0];
        var HT = (Dictionary<string, object>)infoData[1];
        var team = (Define.Team)HT["te"];
        var nickName = (string)HT["nn"];
        var avarerKey = (int)HT["ch"];
        var fogController = this.GetComponentInChildren<FogOfWarController>();
        var playerController = this.GetComponent<PlayerController>();


        var userController = Managers.Game.GetUserController(actorNumber);
        if (userController)
        {
            this.gameObject.tag = "User";
            userController.playerController = playerController;
            this.photonView.ControllerActorNr = actorNumber;
        }
        //없으면AI
        else
        {
            this.gameObject.tag = "AI";
        }

        CreateAvaterByIndex(avarerKey, playerController);
        //playerController.playerStat.SetupCharacter(avater);
        playerController.playerHealth.Team = team;
        playerController.NickName = nickName;       //닉네임 설정
        playerController.OnPhotonInstantiate(this.photonView);
        if (this.IsMyCharacter())
        {
            Managers.CameraManager.cameraState = Define.CameraState.MyPlayer;

        }
    }

    public void OnPreNetDestroy(PhotonView rootView)
    {
        if (Managers.Resource == null) return;
        var playerController = this.GetComponent<PlayerController>();
        playerController.OnPreNetDestroy(rootView);
    }


    #endregion





    #region
    /// <summary>
    /// 해당 버퍼가 없으면 추후 들어오는 플레이어는 AI로 간주
    /// </summary>
    [PunRPC]
    public void BufferedSetupLocalUser(PhotonMessageInfo info)
    {
        //_playerController.ChangePlayerType(Define.PlayerType.User);
        if (info.Sender.IsLocal)
        {
            //방장은 소유권안넘기고 바로 자기자신으로!!
            if (PhotonNetwork.IsMasterClient)
            {
                //Managers.Game.myPlayer = _playerController;
            }
            //방장은 소유권안넘기고 바로 자기자신으로!!
            else
            {
                this.photonView.TransferOwnership(info.Sender);
            }
        }
    }
    void CheckUser(int actorNumber , PlayerController playerController)
    {
        //유저
        if (actorNumber > 0)
        {
            var userController = Managers.Game.GetUserController(actorNumber);
            if (userController)
            {
                userController.playerController = playerController;
                this.photonView.ControllerActorNr = actorNumber;
                //_playerController.ChangePlayerType(Define.PlayerType.User);
            }
            //없으면AI
            else
            {
                //_playerController.ChangePlayerType(Define.PlayerType.AI);

            }
        }
        //AI
        else
        {
            //_playerController.ChangePlayerType(Define.PlayerType.AI);

        }
    }

    public void CreateAvaterByIndex(int index , PlayerController playerController)
    {
        var parent = playerController.fogOfWarController.transform;
        var avater = Managers.Spawn.GetSkinByIndex(Define.ProductType.Character, index, parent);

        playerController.playerStat.SetupCharacterAvater(avater);

    }



    //소유권이 바뀌면!!
    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        //변경된 오브젝트만 실행
        if (targetView.ViewID != this.ViewID()) return;
        if (targetView.ControllerActorNr == previousOwner.ActorNumber)
        {
            //_playerController.ChangePlayerType(Define.PlayerType.AI);

        }
        //_playerController.ChangePlayerType(Define.PlayerType.AI);
        ////방장이 줄떄 => User 자기자신것 찾음
        //if (previousOwner.IsMasterClient )
        //{
        //    if (targetView.ControllerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
        //    {
        //        Managers.Game.myPlayer = _playerController;
        //    }
        //    _playerController.ChangePlayerType(Define.PlayerType.AI);
        //}
        ////방장한테 줄뗴 => 항상 AI 
        //else
        //{
        //    previousOwner.TagObject = null;
        //    _playerController.ChangePlayerType(Define.PlayerType.AI);
        //}
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
    }


    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
    }
    #endregion

}
