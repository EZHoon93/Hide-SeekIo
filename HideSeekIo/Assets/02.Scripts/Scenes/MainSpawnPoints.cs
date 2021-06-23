using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSpawnPoints : MonoBehaviour
{
    [SerializeField] SpawnPoint[] _hiderPoints;
    [SerializeField] SpawnPoint[] _seekerPoints;

    public SpawnPoint[] HiderSpawnPoints => _hiderPoints;
    public SpawnPoint[] SeekerSpawnPoints => _seekerPoints;


    private void Reset()
    {
        _hiderPoints = transform.GetChild(0).GetComponentsInChildren<SpawnPoint>();
        _seekerPoints = transform.GetChild(1).GetComponentsInChildren<SpawnPoint>();

    }


}
