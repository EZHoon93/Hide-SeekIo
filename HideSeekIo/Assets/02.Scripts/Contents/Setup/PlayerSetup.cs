using System.Collections;

using UnityEngine;
using Photon.Pun;
using System;
using FoW;

public class PlayerSetup : MonoBehaviourPun, IPunInstantiateMagicCallback , IOnPhotonInstantiate, IOnPhotonViewPreNetDestroy
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

    
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (photonView.InstantiationData == null) return;   //데이터가없으면 null

        //-데이터받아옴//
        var nickName = (string)info.photonView.InstantiationData[0]; //닉네임
        var avaterId = (string)info.photonView.InstantiationData[1]; //캐릭터 스킨
        var isAI = (bool)info.photonView.InstantiationData[2];  //AI 여부
        var characterType = (Define.CharacterType)info.photonView.InstantiationData[3]; //캐릭터 타
        var fogController = this.GetComponentInChildren<FogOfWarController>();
        var playerController = this.GetComponent<PlayerController>();
        AddInputComponent(isAI);
        playerController.NickName = nickName;       //닉네임 설정
        CreateCharacter(characterType, playerController, fogController);
        //CreaterAvater(avaterId, playerController, fogController);


        playerController.OnPhotonInstantiate(this.photonView);
        _onPhotonInstantiateEvent?.Invoke(this.photonView);
        //유저자신의 캐릭이라면.
        if (this.IsMyCharacter())
        {
            CameraManager.Instance.SetupTarget(playerController.transform);
            SetActiveADNCamera(false);
        }
        else
        {
            
        }

    }

    void AddInputComponent(bool isAI )
    {
       
        InputBase inputBase = null;
        if (isAI)
        {
            this.gameObject.tag = "AI";
            inputBase = this.gameObject.AddComponent<AIInput>();
        }
        else
        {
            this.gameObject.tag = "Player";
            inputBase = this.gameObject.GetOrAddComponent<UserInput>();
        }
        inputBase.OnPhotonInstantiate();
    }

    GameObject CreateCharacter(Define.CharacterType characterType, PlayerController playerController, FogOfWarController fogOfWarController)
    {
        var go = Managers.Spawn.CharacterSpawn(characterType, playerController).GetComponent<Character_Base>();
        go.transform.ResetTransform(this.transform);
        go.OnPhoninstiate(playerController);
        playerController.GetAttackBase().character_Base = go;
        go.animator.runtimeAnimatorController = GameSetting.Instance.GetRuntimeAnimatorController(playerController.Team);
        fogOfWarController._hideInFog.ClearRenders();
        fogOfWarController.AddHideRender(go.GetComponentInChildren<SkinnedMeshRenderer>());

        return go.gameObject;
    }

    //void SetupSkill(Character_Base character_Base , PlayerController playerController)
    //{
    //    playerController.inputBase.mainInput
    //}

    void CreaterAvater(string avaterID, PlayerController playerController, FogOfWarController fogOfWarController )
    {
        var avater = Managers.Resource.Instantiate($"Avater/{avaterID}", this.transform);
        avater.transform.ResetTransform();
        avater.GetOrAddComponent<Animator>().runtimeAnimatorController = GameSetting.Instance.GetRuntimeAnimatorController(playerController.Team);
        fogOfWarController._hideInFog.ClearRenders();
        fogOfWarController.AddHideRender(avater.GetComponentInChildren<SkinnedMeshRenderer>());
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
}
