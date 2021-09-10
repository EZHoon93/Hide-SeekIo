using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Photon.Pun;

using UnityEngine;

public class ItemSpawnController : MonoBehaviour
{
    [SerializeField] GameObject _prefab;
    [SerializeField] SpawnPoint[] _spawnPoints;
    [SerializeField] int _initCount;    //게임 시작 시 한번에 생성할 수 
    [SerializeField] int _maxCount;
    [SerializeField] float _spawnDistance;
    [SerializeField] float _spawnTimeBet;   //간격

    float _canRemainSpawnTime;   //마지막 생성타임
    List<int> _canSpawnIndexList;    //생성가능한 위치
    List<GetWorldItemController> _createWorldItemList;
    HashSet<GetWorldItemController> _createspawnList = new HashSet<GetWorldItemController>();
    public int controllerIndex { get; set; } //스폰매니저에서의 배열인덱스
    public SpawnPoint[] spawnPoints => _spawnPoints;


    private void Awake()
    {
        _spawnPoints = GetComponentsInChildren<SpawnPoint>();
        _canSpawnIndexList = new List<int>(_spawnPoints.Length);
        _createWorldItemList = new List<GetWorldItemController>(_spawnPoints.Length);
    }
    private void Start()
    {
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            _spawnPoints[i].spawnIndex = i;
            _canSpawnIndexList.Add(i);
        }
        _maxCount = _spawnPoints.Length < _maxCount ? _spawnPoints.Length : _maxCount;
    }

    public void Clear()
    {
        if (_canSpawnIndexList.Count > 0)
            _canSpawnIndexList.Clear();
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            _spawnPoints[i].spawnIndex = i;
            _canSpawnIndexList.Add(i);
        }
        foreach (var wc in _createWorldItemList)
        {
            if (wc.photonView.IsMine)
            {
                wc.isReset = true;
                PhotonNetwork.Destroy(wc.gameObject);
            }
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
        int ran = Random.Range(0, _canSpawnIndexList.Count);
        int spawnIndex = _spawnPoints[ran].spawnIndex;
        var pos = _spawnPoints[spawnIndex].transform.position;

        PhotonNetwork.InstantiateRoomObject(_prefab.name ,pos ,Quaternion.identity, 0, new object[] { controllerIndex, spawnIndex }  ); 
    }

    public void CreateCallBack(GetWorldItemController  getWorldItemController)
    {
        _canSpawnIndexList.RemoveAt(getWorldItemController.spawnIndex);
        _createWorldItemList.Add(getWorldItemController);
        //if (_createspawnList.Contains(getWorldItemController) == false)
        //{
        //    _createspawnList.Add(getWorldItemController);
        //}
    }

    public void RemoveCallBack(GetWorldItemController  getWorldItemController)
    {
        _canSpawnIndexList.Add(getWorldItemController.spawnIndex);
        if (_createWorldItemList.Contains(getWorldItemController))
        {
            _createWorldItemList.Remove(getWorldItemController);
        }
        //_createspawnList.Remove(getWorldItemController);


    }



}
