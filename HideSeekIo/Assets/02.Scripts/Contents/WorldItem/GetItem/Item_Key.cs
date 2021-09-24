using System.Collections;
using System.Collections.Generic;

using Photon.Pun;

using UnityEngine;

public class Item_Key : ItemBox_Base
{
    Mission_Key _mission_Key;

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _mission_Key = Managers.Game.CurrentGameScene.gameMissionController.GetComponent<Mission_Key>();
        if (_mission_Key)
        {
            _mission_Key.keyList.Add(this);
        }
    }

    public override void OnPreNetDestroy(PhotonView rootView)
    {
        if (_mission_Key)
        {
            _mission_Key.keyList.Remove(this);
        }
    }
    public override void Get(GameObject getObject)
    {
        EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position, 0);
    }
   
}
