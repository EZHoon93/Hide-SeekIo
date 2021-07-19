using System.Collections;
using System.Collections.Generic;

using Photon.Pun;

using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
	public static int coinCount;	//총 생성된 코인 수 
    
    [SerializeField] SpawnPoint[] _hiderItemPoints;
    [SerializeField] SpawnPoint[] _seekerItemPoints;

    [SerializeField] float _spawnDistance;
    [SerializeField] float _spawnTimeBet;   //간격

    public static List<int> HiderItem_ExistSpawnIndex = new List<int>();
    public static List<int> SeekerItem_ExistSpawnIndex = new List<int>();

    public SpawnPoint[] HiderItemPoints => _hiderItemPoints;
    public SpawnPoint[] SeekerItemPoints => _seekerItemPoints;




    private void Start()
    {
        _hiderItemPoints = transform.GetChild(0).GetComponentsInChildren<SpawnPoint>();
        _seekerItemPoints = transform.GetChild(1).GetComponentsInChildren<SpawnPoint>();

        PhotonGameManager.Instacne.AddListenr(Define.GameState.Gameing, () => StartCoroutine(UpdateSpawn_HiderItem()))  ;
        PhotonGameManager.Instacne.AddListenr(Define.GameState.Gameing, () =>StartCoroutine(UpdateSpawn_SeekerItem())) ;


    }

    IEnumerator UpdateSpawn_HiderItem()
    {
        while (true)
        {
            if (PhotonNetwork.IsMasterClient)
            {

                if (HiderItem_ExistSpawnIndex.Count + 1 < _hiderItemPoints.Length)
                {
                    var spawnIndex = GetSpawnIndex(Define.Team.Hide);
                    var spawnPoint = GetSpawnPoint(Define.Team.Hide, spawnIndex);
                    PhotonNetwork.InstantiateRoomObject("ItemRandomBox", spawnPoint, Quaternion.identity,0,
                        new object[] { Define.Team.Hide, spawnIndex  } );

                    yield return new WaitForSeconds(_spawnTimeBet);
                }
            }

            yield return new WaitForSeconds(2.0f);
        }
    }

    IEnumerator UpdateSpawn_SeekerItem()
    {
        while (true)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                print("생성 !!");
                if (SeekerItem_ExistSpawnIndex.Count + 1 < _seekerItemPoints.Length)
                {
                    var spawnIndex = GetSpawnIndex(Define.Team.Seek);
                    var spawnPoint = GetSpawnPoint(Define.Team.Seek, spawnIndex);
                    PhotonNetwork.InstantiateRoomObject("ItemRandomBox", spawnPoint, Quaternion.identity, 0,
                        new object[] { Define.Team.Seek, spawnIndex });

                    yield return new WaitForSeconds(_spawnTimeBet);
                }
            }

            yield return new WaitForSeconds(2.0f);
        }
    }
    int GetSpawnIndex(Define.Team team)
    {
        int resultSpawnIndex = 0;
        do
        {
            switch (team)
            {
                case Define.Team.Hide:
                    resultSpawnIndex = Random.Range(0, _hiderItemPoints.Length);
                    break;
                case Define.Team.Seek:
                    resultSpawnIndex = Random.Range(0, _seekerItemPoints.Length);
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
                return UtillGame.GetRandomPointOnNavMesh(_hiderItemPoints[spawnIndex].transform.position, _spawnDistance);
            case Define.Team.Seek:
                return UtillGame.GetRandomPointOnNavMesh(_seekerItemPoints[spawnIndex].transform.position, _spawnDistance);
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

