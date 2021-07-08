using System.Collections;

using UnityEngine;

public class HiderRandomBox : MonoBehaviour, IGetWorldItem
{
    


    public void Get(GameObject getObject)
    {
        var playerController =  getObject.GetComponent<PlayerController>();
        if (playerController == null) return;
        Managers.Spawn.InGameItemSpawn(GetRandomItemEnum(), playerController);
    }

    Define.HiderStoreList GetRandomItemEnum()
    {
        var result =  Util.RandomEnum<Define.HiderStoreList>();

        return (Define.HiderStoreList)result;
    }

   
}
