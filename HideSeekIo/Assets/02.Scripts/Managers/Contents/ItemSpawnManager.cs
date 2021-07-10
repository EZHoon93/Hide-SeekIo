using System.Collections;
using System.Collections.Generic;

using Photon.Pun;

using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
	public static int coinCount;	//총 생성된 코인 수 
    
    [SerializeField] SpawnPoint[] hiderItemPoints;
    [SerializeField] SpawnPoint[] seekerItemPoints;

    [SerializeField] float _spawnDistance;
    [SerializeField] float _spawnTimeBet;   //간격

    public static List<int> HiderItem_ExistSpawnIndex = new List<int>();
    public static List<int> SeekerItem_ExistSpawnIndex = new List<int>();



    private void Start()
    {
        hiderItemPoints = transform.GetChild(0).GetComponentsInChildren<SpawnPoint>();
        seekerItemPoints = transform.GetChild(1).GetComponentsInChildren<SpawnPoint>();


        PhotonGameManager.Instacne.GameStartEvent += ()=> StartCoroutine(UpdateSpawn_HiderItem());
        //PhotonGameManager.Instacne.GameStartEvent += () => StartCoroutine(UpdateSpawn_SeekerItem());

    }

    IEnumerator UpdateSpawn_HiderItem()
    {
        while (true)
        {
            if(HiderItem_ExistSpawnIndex.Count + 1 < hiderItemPoints.Length )
            {
                var spawnIndex = GetSpawnIndex(Define.Team.Hide);
                var spawnPoint = GetSpawnPoint(Define.Team.Hide, spawnIndex);
                Managers.Spawn.RoomItemSpawn(Define.RoomItem.HiderRandomBox, spawnPoint, spawnIndex);
                yield return new WaitForSeconds(_spawnTimeBet);
            }

            yield return new WaitForSeconds(2.0f);
        }
    }

    //IEnumerator UpdateSpawn_SeekerItem()
    //{
    //    while (true)
    //    {
    //        if (HiderItem_ExistSpawnIndex.Count + 1 < hiderItemPoints.Length)
    //        {
    //            var resultSpawnIndex = GetSpawnIndex(Define.Team.Hide);
    //            var spawnPoint = GetSpawnPoint(Define.Team.Hide, resultSpawnIndex);
    //            Managers.Spawn.RoomItemSpawn(Define.RoomItem.HiderRandomBox, spawnPoint, 0);
    //            yield return new WaitForSeconds(10.0f);
    //        }

    //        yield return new WaitForSeconds(2.0f);
    //    }
    //}
    int GetSpawnIndex(Define.Team team)
    {
        int resultSpawnIndex = 0;
        do
        {
            switch (team)
            {
                case Define.Team.Hide:
                    resultSpawnIndex = Random.Range(0, hiderItemPoints.Length);
                    break;
                case Define.Team.Seek:
                    resultSpawnIndex = Random.Range(0, seekerItemPoints.Length);
                    break;
            }
        } while ( IsOkSpawnIndex(team , resultSpawnIndex));

        return resultSpawnIndex;
    }

    Vector3 GetSpawnPoint(Define.Team team , int spawnIndex)
    {
        
        switch (team)
        {
            case Define.Team.Hide:
                return UtillGame.GetRandomPointOnNavMesh(hiderItemPoints[spawnIndex].transform.position, _spawnDistance);
            case Define.Team.Seek:
                return UtillGame.GetRandomPointOnNavMesh(seekerItemPoints[spawnIndex].transform.position, _spawnDistance);
        }

        return Vector3.zero;
    }

    bool IsOkSpawnIndex(Define.Team team, int spawnIndex)
    {
        switch (team)
        {
            case Define.Team.Hide:
                return HiderItem_ExistSpawnIndex.Contains(spawnIndex);
            case Define.Team.Seek:
                return HiderItem_ExistSpawnIndex.Contains(spawnIndex);
        }

        return false;
    }

}

