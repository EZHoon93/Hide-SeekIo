using System.Collections;

using Photon.Pun;

using UnityEngine;

public class HiderRandomBox : MonoBehaviour, IGetWorldItem , IPunInstantiateMagicCallback , IOnPhotonViewPreNetDestroy
{
    int _spawnIndex;
    
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        _spawnIndex = (int)info.photonView.InstantiationData[0];

        ItemSpawnManager.HiderItem_ExistSpawnIndex.Add(_spawnIndex);

    }
    public void OnPreNetDestroy(PhotonView rootView)
    {
        ItemSpawnManager.HiderItem_ExistSpawnIndex.Remove(_spawnIndex);

    }

    private void OnDisable()
    {
        ItemSpawnManager.HiderItem_ExistSpawnIndex.Remove(_spawnIndex);

    }

    public void Get(GameObject getObject)
    {
        var playerController =  getObject.GetComponent<PlayerController>();
        if (playerController == null) return;
        //Managers.Spawn.InGameItemSpawn(GetRandomItemEnum(), playerController);
    }

    Define.HiderStoreList GetRandomItemEnum()
    {
        var result =  Util.RandomEnum<Define.HiderStoreList>();

        return (Define.HiderStoreList)result;
    }


}
