using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AIManager : GenricSingleton<AIManager>
{
    [SerializeField] SpawnPoint[] _initAIDestPoint;  //초기 AI 도망갈 위치

    public SpawnPoint[] InitPoint => _initAIDestPoint;

    public Dictionary<int, GameObject> AIDic { get; set; } = new Dictionary<int, GameObject>();
}
