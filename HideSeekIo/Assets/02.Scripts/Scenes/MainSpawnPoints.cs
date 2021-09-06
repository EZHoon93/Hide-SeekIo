using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MainSpawnPoints : MonoBehaviour
{
    [SerializeField] SpawnPoint[] _playerPoints;

    public SpawnPoint[] playerSpawnPoints => _playerPoints;


    private void Reset()
    {
    }

    private void Awake()
    {
        if (_playerPoints.Length == 0)
            _playerPoints = transform.GetChild(0).GetComponentsInChildren<SpawnPoint>();
    }


}
