using System;
using System.Collections;
using Photon.Pun;
using Random = UnityEngine.Random;
using UnityEngine;

public class ItemStatBox : MonoBehaviour, IGetWorldItem, IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy
{
    int _spawnIndex;
    Define.Team _team;
    [SerializeField] GameObject _modelObject;
    [SerializeField] GameObject _aiColliderObject;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        _team = (Define.Team)info.photonView.InstantiationData[0];
        _spawnIndex = (int)info.photonView.InstantiationData[1];


        switch (_team)
        {
            case Define.Team.Hide:
                this.gameObject.layer = (int)Define.Layer.HiderItem;
                this._modelObject.SetLayerRecursively((int)Define.Layer.HiderItem);
                _aiColliderObject.layer = (int)Define.Layer.AIHiderItem;
                ItemSpawnManager.HiderItem_ExistSpawnIndex.Add(_spawnIndex);
                break;
            case Define.Team.Seek:
                this.gameObject.layer = (int)Define.Layer.SeekerItem;
                _aiColliderObject.layer = (int)Define.Layer.AISeekerItem;
                this._modelObject.SetLayerRecursively((int)Define.Layer.SeekerItem);
                ItemSpawnManager.SeekerItem_ExistSpawnIndex.Add(_spawnIndex);
                break;
        }


    }
    public void OnPreNetDestroy(PhotonView rootView)
    {
        switch (_team)
        {
            case Define.Team.Hide:
                ItemSpawnManager.HiderItem_ExistSpawnIndex.Remove(_spawnIndex);
                break;
            case Define.Team.Seek:
                ItemSpawnManager.SeekerItem_ExistSpawnIndex.Remove(_spawnIndex);
                break;
        }
    }

    public void Get(GameObject getObject)
    {
        var playerController = getObject.GetComponent<PlayerController>();
        if (playerController == null) return;
        playerController.playerStat.StatPoint++;
    }

}
