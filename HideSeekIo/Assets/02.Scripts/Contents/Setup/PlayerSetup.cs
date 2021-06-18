using System.Collections;

using UnityEngine;
using Photon.Pun;
using System;

public class PlayerSetup : MonoBehaviourPun, IPunInstantiateMagicCallback
{

    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (photonView.InstantiationData == null) return;   //데이터가없으면 null
        
        var nickName = (string)info.photonView.InstantiationData[0]; //닉네임
        var avaterId = (string)info.photonView.InstantiationData[1]; //캐릭터
        var playerController = this.GetComponent<PlayerController>();
        //-데이터받아옴//
        var avater = Managers.Resource.Instantiate($"Avater/{avaterId}",this.transform);
        avater.transform.localPosition = Vector3.zero;
        avater.transform.localRotation = Quaternion.Euler(Vector3.zero);
        avater.GetOrAddComponent<Animator>().runtimeAnimatorController = GameSetting.Instance.GetRuntimeAnimatorController(playerController.GetTeam());

        //유저자신의 캐릭이라면.
        if (this.IsMyCharacter())
        {
            CameraManager.instance.SetupTarget(playerController);
        }


        playerController.OnPhotonInstantiate();
    }

    
}
