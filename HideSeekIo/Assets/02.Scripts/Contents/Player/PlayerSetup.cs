using System.Collections;

using UnityEngine;
using Photon.Pun;
using System;
using FoW;
using Photon.Realtime;
using UnityStandardAssets.Characters.ThirdPerson.PunDemos;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PhotonView))]
//[RequireComponent(typeof(CharacterController))]

public class PlayerSetup : MonoBehaviourPun, IPunInstantiateMagicCallback , IOnPhotonInstantiate, IPunOwnershipCallbacks, IOnPhotonViewPreNetDestroy,IOnPhotonViewControllerChange,IOnPhotonViewOwnerChange
{
    event Action<PhotonView> _onPhotonInstantiateEvent;
    public event Action<PhotonView> OnPhotonDestroyEvent;
    public event Action<PhotonView> OnPhotonInstantiateEvent
    {
        add
        {
            _onPhotonInstantiateEvent += value;
        }

        remove
        {
            _onPhotonInstantiateEvent -= value;
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (photonView.InstantiationData == null) return;   //데이터가없으면 null
        //-데이터받아옴//
        var nickName = (string)info.photonView.InstantiationData[0]; //닉네임
        var autoNumber = (int)info.photonView.InstantiationData[1]; //로컬넘버
        var characterType = (Define.CharacterType)info.photonView.InstantiationData[2]; //캐릭터 타
        var avaterId = (string)info.photonView.InstantiationData[3]; //캐릭터 스킨
        var fogController = this.GetComponentInChildren<FogOfWarController>();
        var playerController = this.GetComponent<PlayerController>();
        //this.gameObject.tag = autoNumber == -1 ? "AI" : "Player";
        this.gameObject.tag = "AI"; 

        playerController.NickName = nickName;       //닉네임 설정

        playerController.playerCharacter.CreateCharacter(characterType, avaterId);  //캐릭터와 아바타 생성

        playerController.OnPhotonInstantiate(this.photonView);
        _onPhotonInstantiateEvent?.Invoke(this.photonView);
        //info.Sender.TagObject = this;

        //소유권을 갖게되는 플레이어는 RPC를 통해 버퍼로 실행
        if (autoNumber == PhotonNetwork.LocalPlayer.ActorNumber && autoNumber != -1)
        {
            photonView.RPC("SetupUserPlayer", RpcTarget.AllBuffered);
        }
    }

    /// <summary>
    /// 해당 버퍼가 없으면 추후 들어오는 플레이어는 AI로 간주
    /// </summary>
    [PunRPC]
    public void SetupUserPlayer(PhotonMessageInfo info)
    {
        info.Sender.TagObject = this.gameObject;
        this.gameObject.tag = "Player";
        if (info.Sender.IsLocal)
        {
            this.photonView.TransferOwnership(info.Sender.ActorNumber);
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
            CameraManager.Instance.FindNextPlayer();    //다른 유저 관전
        }
    }



    void SetActiveADNCamera(bool active)
    {
        var mainUI = Managers.UI.SceneUI as UI_Main;
        mainUI.FindButton.gameObject.SetActive(active);
        mainUI.ADButton.gameObject.SetActive(active);
    }


    public void OnPreNetDestroy(PhotonView rootView)
    {
        if (Managers.Resource == null) return;
        Managers.Game.UnRegisterLivingEntity(this.ViewID());
        Managers.Resource.Destroy(GetComponentInChildren<Animator>().gameObject);
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
        if (CameraManager.Instance.cameraTagerPlayer.ViewID() == this.ViewID())
        {
            CameraManager.Instance.FindNextPlayer();
        }
        //if(PhotonGameManager.Instacne.State == Define.GameState.Gameing)
        //{
        //    PhotonGameManager.Instacne.GetComponent<GameState_Gameing>().UpdatePlayerCount();
        //}

        if (this.IsMyCharacter())
        {
            SetActiveADNCamera(true);
        }
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        print("요청");
    }

    //전체 이 인터페이스를 가진 오브젝트 전체실행..
    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        //변경된 오브젝트만 실행
        if (targetView.ViewID != this.ViewID()) return;
        PlayerController playerController = GetComponent<PlayerController>();

        //방장으로 부터 소유권을 넘겨받은 AI가 아닌 캐릭터라면.
        if (this.IsMyCharacter())
        {
            playerController.ChangeOwnerShipOnUser();
            CameraManager.Instance.SetupcameraTagerPlayer(playerController.transform);
            SetActiveADNCamera(false);
        }
        //타 유저 => 방장으로 넘겨받았을 경우 AI로 전환
        if(previousOwner.IsMasterClient == false && targetView.Controller.IsMasterClient)
        {
            playerController.ChangeAI();
        }
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        print("실패");
    }

    public void OnControllerChange(Player newController, Player previousController)
    {
        print("OnControllerChange");
    }

    public void OnOwnerChange(Player newOwner, Player previousOwner)
    {
         print("OnOwnerChange");
    }



    //public void OnOwnerChange(Player newOwner, Player previousOwner)
    //{
    //    print("OnOwnerChange");
    //}
}
