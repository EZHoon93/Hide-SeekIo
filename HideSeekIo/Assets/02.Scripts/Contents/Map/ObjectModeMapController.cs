using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ObjectModeMapController : MapController
{
    [SerializeField] ObjectModeController[] _changeObjectList;
    [SerializeField] ObjectModeController _objectModeController;
    public ObjectModeController changeObjectList => _objectModeController;

    List<int> _mapIndexList = new List<int>(256);
    public override void InitMapMaker()
    {
        _mapIndexList.Clear();

        foreach (var o in _changeObjectList)
        {
            var selectIndex = o.MakeRandom();
            _mapIndexList.Add(selectIndex);
        }

        Managers.photonGameManager.SendEvent(Define.PhotonOnEventCode.InitMapObject, EventCaching.AddToRoomCacheGlobal, _mapIndexList.ToArray());
    }


    public override void MapMake(int[] indexArray)
    {
        int i = 0;
        foreach (var index in indexArray)
        {
            _changeObjectList[i].Setup(index);
            i++;
        }
    }
   
}
