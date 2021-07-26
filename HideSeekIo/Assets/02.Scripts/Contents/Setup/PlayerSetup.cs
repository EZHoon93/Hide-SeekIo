using System.Collections;

using UnityEngine;
using Photon.Pun;
using System;
using FoW;

public class PlayerSetup : MonoBehaviourPun, IPunInstantiateMagicCallback , IOnPhotonInstantiate, IOnPhotonViewPreNetDestroy
{
    [SerializeField] GameObject go;
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
        var avaterId = (string)info.photonView.InstantiationData[1]; //캐릭터
        var fogController = this.GetComponentInChildren<FogOfWarController>();
        var playerController = this.GetComponent<PlayerController>();
        playerController.NickName = nickName;       //닉네임 설정

        CreaterAvater(avaterId, playerController, fogController);

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

    void CreaterAvater(string avaterID, PlayerController playerController, FogOfWarController fogOfWarController )
    {
        var avater = Managers.Resource.Instantiate($"Avater/{avaterID}", this.transform);
        avater.transform.ResetTransform();
        avater.GetOrAddComponent<Animator>().runtimeAnimatorController = GameSetting.Instance.GetRuntimeAnimatorController(playerController.Team);
        go = avater;
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
        if(CameraManager.Instance.Target.ViewID() == this.ViewID())
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
