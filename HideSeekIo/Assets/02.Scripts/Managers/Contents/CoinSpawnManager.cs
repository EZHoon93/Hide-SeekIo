using System.Collections;

using Photon.Pun;

using UnityEngine;

public class CoinSpawnManager : MonoBehaviour
{
	public static int coinCount;	//총 생성된 코인 수 
    GameObject coinPrefab;
    [SerializeField] SpawnPoint[] spawnPoints;
    [SerializeField] float _spawnDistance;
    int lastSpawnIndex;


    
    private void Start()
    {
        coinPrefab = Managers.Resource.Load<GameObject>("Prefabs/Photon/ItemCoin");
        spawnPoints = GetComponentsInChildren<SpawnPoint>();
        PhotonGameManager.Instacne.GameStartEvent += ()=> StartCoroutine(UpdateSpawnCoin());
	}

	IEnumerator UpdateSpawnCoin()
    {
        while (true)
        {
            if(coinCount < 4)
            {
                var spawnPoint = GetSpawnPoint();
                PhotonNetwork.InstantiateRoomObject(coinPrefab.name, spawnPoint, Quaternion.identity);
                yield return new WaitForSeconds(10.0f);
            }
        }
    }


    Vector3 GetSpawnPoint()
    {
        int ran = 0;
        Vector3 reuslt = Vector3.zero;
        do
        {
            ran = Random.Range(0, spawnPoints.Length);
        } while (lastSpawnIndex == ran);

        reuslt =  UtillGame.GetRandomPointOnNavMesh(spawnPoints[ran].transform.position, _spawnDistance);
        reuslt.y += 2;
        return reuslt;
    }
}

