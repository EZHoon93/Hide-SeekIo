using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class MainSpawnPoints : MonoBehaviour
{
    [SerializeField] SpawnPoint[] _hiderPlayerPoints;
    [SerializeField] SpawnPoint[] _seekerPoints;

    public SpawnPoint[] hiderPlayerPoints => _hiderPlayerPoints;
    public SpawnPoint[] seekerPoints => _seekerPoints;


    List<SpawnPoint> _hiderSpawnPointCopyList = new List<SpawnPoint>(32);
    List<SpawnPoint> _seekerSpawnPointCopyList = new List<SpawnPoint>(8);

    private void Reset()
    {
      
    }

    private void Awake()
    {
        if (_hiderPlayerPoints.Length == 0)
            _hiderPlayerPoints = transform.GetChild(0).GetComponentsInChildren<SpawnPoint>();

        if (_seekerPoints.Length == 0)
            _seekerPoints = transform.GetChild(1).GetComponentsInChildren<SpawnPoint>();


        for (int i = 0; i < _hiderPlayerPoints.Length; i++)
        {
            _hiderPlayerPoints[i].spawnIndex = i;
        }
        for (int i = 0; i < _seekerPoints.Length; i++)
        {
            _seekerPoints[i].spawnIndex = i;
        }
    }

    private void Start()
    {
        Managers.Game.AddListenrOnGameState(Define.GameState.Wait, Clear);
    }

    void Clear()
    {
        _hiderSpawnPointCopyList.Clear();
    }

    public Vector3 GetHiderPosition_Random()
    {
        if(_hiderSpawnPointCopyList.Count <= 0 )
        {
            _hiderSpawnPointCopyList = _hiderPlayerPoints.ToList();
        }

        var ran = Random.Range(0, _hiderSpawnPointCopyList.Count);
        var result = _hiderSpawnPointCopyList[ran];
        _hiderSpawnPointCopyList.RemoveAt(ran);

        return result.transform.position;
    }


    public Vector3 GetSeekerPosition_Random()
    {
        if (_seekerSpawnPointCopyList.Count <= 0)
        {
            _seekerSpawnPointCopyList = _seekerPoints.ToList();
        }

        var ran = Random.Range(0, _seekerSpawnPointCopyList.Count);
        var result = _seekerSpawnPointCopyList[ran];
        _seekerSpawnPointCopyList.RemoveAt(ran);
        return result.transform.position;
    }



}
