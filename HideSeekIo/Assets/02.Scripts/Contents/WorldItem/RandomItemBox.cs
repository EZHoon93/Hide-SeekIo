using System;
using System.Collections;

using Photon.Pun;
using Random = UnityEngine.Random;
using UnityEngine;

public class RandomItemBox : MonoBehaviour, IGetWorldItem, IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy
{
    int _spawnIndex;
    Define.Team _team;
    [SerializeField] GameObject _modelObject;
    [SerializeField] GameObject _aiColliderObject;

    //public static Enum[] hiderItemArray = {
    //    Define.ThrowItem.TNT , Define.ThrowItem.Grenade , Define.ThrowItem.Glue,Define.InGameItem.Trap,
    //    Define.InGameItem.Vaccine, Define.InGameItem.Speed, Define.InGameItem.SightUp,
    //    Define.InGameItem.Stealth
    //};

    //public static Enum[] seekerItemArray =
    //{
    //    Define.ThrowItem.Dynamite , Define.ThrowItem.Flash , Define.ThrowItem.PoisonBomb , Define.InGameItem.Speed2,
    //    Define.InGameItem.SightUp, Define.InGameItem.Immune
    //};
    public static Enum[] hiderItemArray = {
          Define.ThrowItem.Flash
    };
    public static Enum[] seekerItemArray =
   {
        Define.ThrowItem.Flash 
    };


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
        var selectItemEnum = GetRandomItemEnum(playerController.Team);
        Managers.Spawn.ItemSpawn(selectItemEnum, playerController);
    }

    Enum GetRandomItemEnum(Define.Team team)
    {
        switch (team)
        {
            case Define.Team.Hide:
                int hiderRandom = Random.Range(0, hiderItemArray.Length);
                return hiderItemArray[hiderRandom];
            case Define.Team.Seek:
                int seekerRandom = Random.Range(0, seekerItemArray.Length);
                return seekerItemArray[seekerRandom];
        }

        return Define.InGameItem.Null;
    }


}
