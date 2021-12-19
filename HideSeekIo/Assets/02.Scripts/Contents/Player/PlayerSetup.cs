
using System;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PhotonView))]
public class PlayerSetup : MonoBehaviourPun,   IPunOwnershipCallbacks , IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy
{
    [SerializeField] PlayerObjectController _playerObjectController;
    [SerializeField] MoveEnergyController _moveEnergyController;


    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    /// <summary>
    ///["nu"] = player.ActorNumber,        //넘버
    //["nn"] = player.NickName ,          //닉네임
    //["te"] = Define.Team.Hide,           //팀
    //["ch"] = HT["ch"],                          //캐릭스킨
    //["we"] = HT["we"],                        //무기스킨
    //["ac"] = HT["ac"],                        //악세스킨
    /// </summary>
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var infoData = info.photonView.InstantiationData;
        if (infoData == null) return;   //데이터가없으면 null
        //-데이터받아옴//
        var autoNumber = (int)infoData[0];
        var HT = (Dictionary<string, object>)infoData[1];
        var team = (Define.Team)HT["te"];
        var nickName = (string)HT["nn"];
        var avarerKey = (int)HT["ch"];
        //var avaterId = (string)info.photonView.InstantiationData[3]; //캐릭터 스킨
        //var weaponId= (string)info.photonView.InstantiationData[4]; //캐릭터 스킨
        //var accesoryId = (string)info.photonView.InstantiationData[5]; //캐릭터 스킨

        var fogController = this.GetComponentInChildren<FogOfWarController>();
        var playerController = this.GetComponent<PlayerController>();

        this.gameObject.tag = "AI";
        playerController.playerHealth.Team = team;
        playerController.NickName = nickName;       //닉네임 설정
        playerController.playerCharacter.CreateAvaterByIndex(avarerKey);  //캐릭터와 아바타 생성
        playerController.OnPhotonInstantiate(this.photonView);

        Util.SetLayerRecursively(fogController.gameObject, UtillLayer.SetupLayerByTeam(team));  //레이어변경.

        //소유권을 갖게되는 플레이어는 RPC를 통해 버퍼로 실행
        if (autoNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            photonView.RPC("SetupUserPlayer", RpcTarget.AllBuffered);
        }

        if (photonView.IsMine && autoNumber < 0)
        {
            playerController.ChangeOwnerShipOnUser(false);
        }


    }

    /// <summary>
    /// 해당 버퍼가 없으면 추후 들어오는 플레이어는 AI로 간주
    /// </summary>
    [PunRPC]
    public void SetupUserPlayer(PhotonMessageInfo info)
    {
        this.gameObject.tag = "Player";
        if (info.Sender.IsLocal)
        {
            this.photonView.TransferOwnership(info.Sender.ActorNumber);
        }
    }

    void CheckGameMode()
    {
        switch(Managers.Scene.currentGameScene.gameMode)
        {
            case Define.GameMode.Object:

                break;
        }
    }

   

    public void RemoveUserPlayerToServer()
    {
        photonView.RPC("RemoveUserPlayerOnServer", RpcTarget.All);
    }
    /// <summary>
    /// 수동으로 게임 나갔을 시 AI로 전환
    /// </summary>
    [PunRPC]
    public void RemoveUserPlayerOnServer(PhotonMessageInfo info)
    {
        info.Sender.TagObject = null;
        this.gameObject.tag = "AI";
        if (info.Sender.IsLocal)
        {
            PhotonNetwork.RemoveRPCs(this.photonView);
            this.photonView.TransferOwnership(0);   //AI이므로 방장한테 넘김
            Managers.cameraManager.FindNextPlayer();    //다른 유저 관전
        }
    }



    void SetActiveADNCamera(bool active)
    {
        var mainUI = Managers.UI.SceneUI as UI_Main;
        if (mainUI)
        {
            //mainUI.FindButton.gameObject.SetActive(active);
            //mainUI.ADButton.gameObject.SetActive(active);
        }
        
    }


    public void OnPreNetDestroy(PhotonView rootView)
    {
        if (Managers.Resource == null) return;
        Managers.Game.UnRegisterLivingEntity(this.ViewID());
        //Managers.Resource.Destroy(GetComponentInChildren<Animator>().gameObject);
        var behavorTree = GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
        if (behavorTree)
        {
            Destroy(behavorTree);
        }
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent)
        {
            Destroy(agent);
        }
        //현재 카메라가 보고있는것이 파괴되엇다면
        //if (Managers.cameraManager.cameraTagerPlayer.ViewID() == this.ViewID())
        //{
        //    Managers.cameraManager.FindNextPlayer();
        //}
        //if(Managers.photonGameManager.State == Define.GameState.Gameing)
        //{
        //    Managers.photonGameManager.GetComponent<GameState_Gameing>().UpdatePlayerCount();
        //}
        this.GetComponent<PlayerController>().OnPreNetDestroy(rootView);


        if (this.IsMyCharacter())
        {
            SetActiveADNCamera(true);
        }
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
    }


    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
    }



    //전체 이 인터페이스를 가진 오브젝트 전체실행..
    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        //변경된 오브젝트만 실행
        if (targetView.ViewID != this.ViewID()) return;
        PlayerController playerController = GetComponent<PlayerController>();
        var isMyCharacter = this.IsMyCharacter();
        playerController.ChangeOwnerShipOnUser(isMyCharacter);
        //방장으로 부터 소유권을 넘겨받은 AI가 아닌 캐릭터라면.
        if (isMyCharacter)
        {
            Managers.cameraManager.SetupcameraTagerPlayer(playerController.transform);
            SetActiveADNCamera(false);
        }
        //타 유저 => 방장으로 넘겨받았을 경우 AI로 전환
        if (previousOwner.IsMasterClient == false && targetView.Controller.IsMasterClient)
        {
            playerController.ChangeAI();
        }
        //양도 받았는데 이미 죽은캐릭터라면 삭제
        if (photonView.IsMine && playerController.playerHealth.Dead)
        {

            playerController.playerHealth.Invoke("AfterDestory", 3.0f);
        }
    }

}
