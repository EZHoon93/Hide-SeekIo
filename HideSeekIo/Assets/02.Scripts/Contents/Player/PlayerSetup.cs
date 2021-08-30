using System.Collections;

using UnityEngine;
using Photon.Pun;
using System;
using FoW;
using Photon.Realtime;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(CharacterController))]

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
        var isAI = (bool)info.photonView.InstantiationData[4];  //AI 여부
        var fogController = this.GetComponentInChildren<FogOfWarController>();
        var playerController = this.GetComponent<PlayerController>();


        this.gameObject.tag = isAI ? "AI" : "Player";

        playerController.NickName = nickName;       //닉네임 설정
        Component c = GetComponent<Character_Base>();
        if (c)
        {
            Destroy(c);
        }

        playerController.character_Base = GetOrAddCharacterComponent(characterType);
        CreateCharacterAvater(characterType, avaterId,playerController, fogController);
        playerController.OnPhotonInstantiate(this.photonView);
        _onPhotonInstantiateEvent?.Invoke(this.photonView);

        //유저자신의 캐릭이라면.
        if (autoNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            //if (PhotonNetwork.IsMasterClient == false) return;
            print("넘기기.." + autoNumber);
            this.photonView.TransferOwnership(autoNumber);
            //this.photonView.contr
         
        }
        else
        {
            print("안넘기");
        }

    }



    CharacterAvater CreateCharacterAvater(Define.CharacterType characterType, string avaterID, PlayerController playerController, FogOfWarController fogOfWarController)
    {
        var characterAvater = Managers.Spawn.CharacterAvaterSpawn(characterType,avaterID).GetComponent<CharacterAvater>();
        characterAvater.transform.ResetTransform(this.transform);
        playerController.character_Base.characterAvater = characterAvater;
        //playerController.character_Base = character_Base;
        //character_Base.playerController = playerController;
        //character_Base.CreateAvater(avaterID);
        //go.animator.runtimeAnimatorController = GameSetting.Instance.GetRuntimeAnimatorController(playerController.Team);
        fogOfWarController._hideInFog.ClearRenders();
        //fogOfWarController.AddHideRender(character_Base.renderers);

        return characterAvater;
    }

    //void SetupSkill(Character_Base character_Base , PlayerController playerController)
    //{
    //    playerController.inputBase.mainInput
    //}

    //void CreaterAvater(string avaterID, PlayerController playerController, FogOfWarController fogOfWarController )
    //{
    //    var avater = Managers.Resource.Instantiate($"Character/{avaterID}", this.transform).GetComponent<CharacterAvater>();
    //    avater.transform.ResetTransform();
    //    print(avater + "아바타");
    //    avater.animator.runtimeAnimatorController = GameSetting.Instance.GetRuntimeAnimatorController(playerController.Team);
    //    fogOfWarController._hideInFog.ClearRenders();
    //    fogOfWarController.AddHideRender(avater.GetComponentInChildren<SkinnedMeshRenderer>());
    //}

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
        var inputBase = GetComponent<InputBase>();
        if (inputBase)
        {
            Destroy(inputBase);
        }
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
        if (CameraManager.Instance.Target.ViewID() == this.ViewID())
        {
            CameraManager.Instance.FindNextPlayer();
        }
        if(PhotonGameManager.Instacne.State == Define.GameState.Gameing)
        {
            PhotonGameManager.Instacne.GetComponent<GameState_Gameing>().UpdatePlayerCount();
        }

        if (this.IsMyCharacter())
        {
            SetActiveADNCamera(true);
        }
    }

    Character_Base GetOrAddCharacterComponent(Define.CharacterType characterType)
    {
        Character_Base character_Base ;
        switch (characterType)
        {
            case Define.CharacterType.Bear:
                character_Base = this.gameObject.GetOrAddComponent<Character_Bear>();
                break;
            case Define.CharacterType.Bunny:
                character_Base = this.gameObject.GetOrAddComponent<Character_Bunny>();

                break;
            case Define.CharacterType.Cat:
                character_Base = this.gameObject.GetOrAddComponent<Character_Cat>();

                break;
            //case Define.CharacterType.Dog:
            //    character_Base = this.gameObject.GetOrAddComponent<Character_Dog>();

            //    break;
            //case Define.CharacterType.Frog:
            //    character_Base = this.gameObject.GetOrAddComponent<Character_Frog>();

            //    break;
            //case Define.CharacterType.Monkey:
            //    character_Base = this.gameObject.GetOrAddComponent<Character_Monkey>();

                //break;
            default:
                character_Base = this.gameObject.GetOrAddComponent<Character_Bear>();
                break;
        }

        return character_Base;
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        print("요청");
    }

    //전체 이 인터페이스를 가진 오브젝트 전체실행..
    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView.ViewID != this.ViewID()) return;
        if (this.gameObject.IsValidAI())
        {

        }
        else
        {
            if (this.photonView.IsMine)
            {
                PlayerController playerController = GetComponent<PlayerController>();
                playerController.ChangeOwnerShip();
                CameraManager.Instance.SetupTarget(playerController.transform);
                SetActiveADNCamera(false);
            }
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
