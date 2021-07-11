﻿using System.Collections;

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
        print(this.gameObject.name + "추가할끄야");
        fogOfWarController.AddHideRender(avater.GetComponentInChildren<SkinnedMeshRenderer>());
    }


    public void OnPreNetDestroy(PhotonView rootView)
    {
        if (Managers.Resource == null) return;
        Managers.Resource.Destroy(GetComponentInChildren<Animator>().gameObject);
        //OnPhotonDestroyEvent?.Invoke();

    }
}
