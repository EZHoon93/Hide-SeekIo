using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Data;

using Photon.Pun;

using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    [SerializeField]  ItemSpawnController[] _itemSpawnControllers;
    [SerializeField] SpawnPoint[] _initSpawnPoints;

    private float updateCheckTimeBet = 1.0f;
    List<SpawnPoint> _canSpawnPointList = new List<SpawnPoint>(32);


    public SpawnPoint[] initSpawnPoints => _initSpawnPoints;


    private void Start()
    {
        if(_canSpawnPointList.Count == 0)
        {
            _canSpawnPointList = new List<SpawnPoint>(32);
        }
        for (int i = 0; i < _itemSpawnControllers.Length; i++)
        {
            _itemSpawnControllers[i].getSpawnEvent = GetSpawnPoint;
            _itemSpawnControllers[i].controllerIndex = i;

        }
        _canSpawnPointList.Clear();
        Managers.Game.AddListenrOnGameState(Define.GameState.Gameing, () => StartCoroutine(UpdateSpawn()));
        Managers.Game.AddListenrOnGameState(Define.GameState.Wait, Clear);

    }

    public void Clear()
    {
        StopAllCoroutines();
        foreach (var sp in _itemSpawnControllers)
            sp.Clear();
    }

    IEnumerator UpdateSpawn()
    {
        while (true)
        {
            //if (PhotonNetwork.IsMasterClient)
            //{
            //    //마스터 클라이언트만 실행
            //    foreach (var isp in _itemSpawnControllers)
            //    {
            //        isp.UpdateSpawn(updateCheckTimeBet);
            //    }
            //}
            yield return new WaitForSeconds(updateCheckTimeBet);
        }
    }


    public SpawnData GetSpawnPoint()
    {
        if (_canSpawnPointList.Count == 0)
        {
            _canSpawnPointList = initSpawnPoints.ToList();
        }
        var ran = Random.Range(0, _canSpawnPointList.Count);
        int spawnIndex = _canSpawnPointList[ran].spawnIndex;
        var spawpnPos = UtillGame.GetRandomPointOnNavMesh(_canSpawnPointList[ran].transform.position, 4);
        SpawnData spawnData;
        spawnData.spawnIndex = spawnIndex;
        spawnData.spawnPos = spawpnPos;

        return spawnData;

    }


    public void CreateCallBack(GetWorldItemController getWorldItemController)
    {
        if (_canSpawnPointList.Count == 0)
        {
            _canSpawnPointList = initSpawnPoints.ToList();
        }
        var spawnPoint = _canSpawnPointList[getWorldItemController.spawnIndex];
        _canSpawnPointList.Remove(spawnPoint);
        int controllerIndex = getWorldItemController.controllerIndex;
        if (controllerIndex >= 0)
        {
            _itemSpawnControllers[controllerIndex].CreateCallBack(getWorldItemController);
        }
    }

    public void RemoveCallBack(GetWorldItemController getWorldItemController)
    {
        var spawnPoint = _canSpawnPointList[getWorldItemController.spawnIndex];
        _canSpawnPointList.Add(spawnPoint);
        int controllerIndex = getWorldItemController.controllerIndex;
        if (controllerIndex >= 0)
        {
            _itemSpawnControllers[controllerIndex].RemoveCallBack(getWorldItemController);

        }
    }

}

