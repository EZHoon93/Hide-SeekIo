using System.Collections;

using UnityEngine;

public class SeekerRandomBox : MonoBehaviour, IGetWorldItem
{



    public void Get(GameObject getObject)
    {
        var playerController = getObject.GetComponent<PlayerController>();
        if (playerController == null) return;
        //Managers.Spawn.InGameItemSpawn(GetRandomItemEnum(), playerController);
    }

    //Define.SeekrStoreList GetRandomItemEnum()
    //{
    //    var result = Util.RandomEnum<Define.SeekrStoreList>();

    //    return (Define.SeekrStoreList)result;
    //}


}
