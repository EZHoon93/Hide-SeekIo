using System.Collections;
using System.Collections.Generic;

using Photon.Pun;

using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
     [SerializeField]  ItemSpawnController[] _itemSpawnControllers;

    private float updateCheckTimeBet = 1.0f;  

    //GetWorldItemController => CallBack;
    public void CreateCallBack(GetWorldItemController getWorldItemController)
    {
        _itemSpawnControllers[getWorldItemController.controllerIndex].CreateCallBack(getWorldItemController);
    }

    public void RemoveCallBack(GetWorldItemController getWorldItemController)
    {
        _itemSpawnControllers[getWorldItemController.controllerIndex].RemoveCallBack(getWorldItemController);

    }



    private void Start()
    {
        for (int i = 0; i < _itemSpawnControllers.Length; i++)
            _itemSpawnControllers[i].controllerIndex = i;
        
        PhotonGameManager.Instacne.AddListenr(Define.GameState.Gameing, () => StartCoroutine(UpdateSpawn()));
        PhotonGameManager.Instacne.AddListenr(Define.GameState.Wait, () => Clear());
    }

    public void Clear()
    {
        print("ITemClear!!");
        StopAllCoroutines();

        foreach (var sp in _itemSpawnControllers)
            sp.Clear();

    }

    IEnumerator UpdateSpawn()
    {
        while (true)
        {
            if (!PhotonNetwork.IsMasterClient) continue;
            //마스터 클라이언트만 실행
            print("생..성 ..SpawnManager");
            foreach(var isp in _itemSpawnControllers)
            {
                isp.UpdateSpawn(updateCheckTimeBet);
            }

            yield return new WaitForSeconds(updateCheckTimeBet);
        }
    }

    //IEnumerator UpdateSpawn_HiderItem()
    //{
    //    while (true)
    //    {
    //        if (PhotonNetwork.IsMasterClient)
    //        {

    //            if (HiderItem_ExistSpawnIndex.Count + 1 < _hiderItemPoints.Length)
    //            {
    //                var spawnIndex = GetSpawnIndex(Define.Team.Hide);
    //                var spawnPoint = GetSpawnPoint(Define.Team.Hide, spawnIndex);
    //                PhotonNetwork.InstantiateRoomObject("ItemRandomBox", spawnPoint, Quaternion.identity, 0,
    //                    new object[] { Define.Team.Hide, spawnIndex });

    //                yield return new WaitForSeconds(_spawnTimeBet);
    //            }
    //        }

    //        yield return new WaitForSeconds(2.0f);
    //    }
    //}

    //IEnumerator UpdateSpawn_SeekerItem()
    //{
    //    while (true)
    //    {
    //        if (PhotonNetwork.IsMasterClient)
    //        {
    //            if (SeekerItem_ExistSpawnIndex.Count + 1 < _seekerItemPoints.Length)
    //            {
    //                var spawnIndex = GetSpawnIndex(Define.Team.Seek);
    //                var spawnPoint = GetSpawnPoint(Define.Team.Seek, spawnIndex);
    //                PhotonNetwork.InstantiateRoomObject(
    //                    "ItemRandomBox", spawnPoint, Quaternion.identity, 0, new object[] { Define.Team.Seek, spawnIndex });

    //                yield return new WaitForSeconds(_spawnTimeBet);
    //            }
    //        }

    //        yield return new WaitForSeconds(2.0f);
    //    }
    //}
    //int GetSpawnIndex(Define.Team team)
    //{
    //    int resultSpawnIndex = 0;
    //    do
    //    {
    //        switch (team)
    //        {
    //            case Define.Team.Hide:
    //                resultSpawnIndex = Random.Range(0, _hiderItemPoints.Length);
    //                break;
    //            case Define.Team.Seek:
    //                resultSpawnIndex = Random.Range(0, _seekerItemPoints.Length);
    //                break;
    //        }
    //    } while (IsOkSpawnIndex(team, resultSpawnIndex));

    //    return resultSpawnIndex;
    //}

    //Vector3 GetSpawnPoint(Define.Team team, int spawnIndex)
    //{
    //    var result = Vector3.zero;
    //    if (team == Define.Team.Hide)
    //    {
    //        result = UtillGame.GetRandomPointOnNavMesh(_hiderItemPoints[spawnIndex].transform.position, _spawnDistance);
    //    }
    //    else
    //    {
    //       result = UtillGame.GetRandomPointOnNavMesh(_seekerItemPoints[spawnIndex].transform.position, _spawnDistance);
    //    }

    //    result.y = 0;
    //    return result;
    //}

    //bool IsOkSpawnIndex(Define.Team team, int spawnIndex)
    //{
    //    switch (team)
    //    {
    //        case Define.Team.Hide:
    //            return HiderItem_ExistSpawnIndex.Contains(spawnIndex);
    //        case Define.Team.Seek:
    //            return HiderItem_ExistSpawnIndex.Contains(spawnIndex);
    //    }

    //    return false;
    //}

}

