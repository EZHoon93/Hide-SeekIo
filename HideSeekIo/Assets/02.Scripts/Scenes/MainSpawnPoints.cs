using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MainSpawnPoints : MonoBehaviour
{
    [SerializeField] SpawnPoint[] _hiderPoints;
    [SerializeField] SpawnPoint[] _seekerPoints;
    //[SerializeField] SpawnPoint[]  

    public SpawnPoint[] HiderSpawnPoints => _hiderPoints;
    public SpawnPoint[] SeekerSpawnPoints => _seekerPoints;


    private void Reset()
    {
        _hiderPoints = transform.GetChild(0).GetComponentsInChildren<SpawnPoint>();
        _seekerPoints = transform.GetChild(1).GetComponentsInChildren<SpawnPoint>();
    }

    private void Awake()
    {
        if (_hiderPoints.Length == 0)
            _hiderPoints = transform.GetChild(0).GetComponentsInChildren<SpawnPoint>();
        if (_seekerPoints.Length == 0)
            _seekerPoints = transform.GetChild(1).GetComponentsInChildren<SpawnPoint>();

    }


}
