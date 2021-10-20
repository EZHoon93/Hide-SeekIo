using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Data;

using Photon.Pun;

using UnityEngine;

public class ItemSpawnController : MonoBehaviour
{
    [SerializeField] GameObject _prefab;
    [SerializeField] int _initCount;    //게임 시작 시 한번에 생성할 수 
    [SerializeField] int _maxCount;
    [SerializeField] float _spawnDistance;
    [SerializeField] float _spawnTimeBet;   //간격

    float _canRemainSpawnTime;   //마지막 생성타임 or 누군가 얻은 마지막시간

    public int controllerIndex { get; set; }
    public delegate SpawnData delegateSpawnPoint();
    public delegateSpawnPoint getSpawnEvent;
    //public Func<SpawnData> func;    //getspawnEvent랑 같은방식
    [SerializeField] List<GetWorldItemController> _createWorldItemList = new List<GetWorldItemController>(20);



    public void Clear()
    {
        foreach(var c  in _createWorldItemList.ToArray())
        {
            if (c == null) continue;
            Managers.Resource.PunDestroy(c);
        }
        _createWorldItemList.Clear();
    }

    public void UpdateSpawn(float intervalTime)
    {
        _canRemainSpawnTime -= intervalTime;
        if (_canRemainSpawnTime > 0 || _createWorldItemList.Count >= _maxCount) return;
        CreateSpawn();
    }
    private void CreateSpawn()
    {
        _canRemainSpawnTime = _spawnTimeBet;
        var spawnData = getSpawnEvent();    //랜덤위치,및 랜덤위치의 인덱스정보 가져옴.
        PhotonNetwork.InstantiateRoomObject(_prefab.name ,spawnData.spawnPos  ,Quaternion.identity, 0, new object[] { spawnData.spawnIndex, controllerIndex }  ); 
    }

   


    public void CreateCallBack(GetWorldItemController getWorldItemController)
    {
        if (_createWorldItemList.Contains(getWorldItemController) == false)
        {
            _createWorldItemList.Add(getWorldItemController);
        }
    }

    public void RemoveCallBack(GetWorldItemController getWorldItemController)
    {
        if (_createWorldItemList.Contains(getWorldItemController))
        {
            _createWorldItemList.Remove(getWorldItemController);
            _canRemainSpawnTime = _spawnTimeBet;
        }
    }



}
