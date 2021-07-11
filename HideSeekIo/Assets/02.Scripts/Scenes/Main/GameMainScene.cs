using System.Collections;
using System.Collections.Generic;

using Photon.Pun;

using UnityEngine;

public class GameMainScene : GameScene
{
    [SerializeField] MainSpawnPoints _mainSpawnPoints;

    public MainSpawnPoints mainSpawnPoints => _mainSpawnPoints;
    protected override void Init()
    {
        base.Init();
        Managers.UI.ShowSceneUI<UI_Main>(); //메인 UI온 
        Managers.Game.CurrentGameScene = this;
    }

    
    public override void Clear()
    {
        
    }

    public Vector3 GetHiderPosition(int index)
    {
        if(_mainSpawnPoints.HiderSpawnPoints.Length <= index)
        {
            Debug.LogError("Hider 스폰포인트 위치가 더작음");
            return Vector3.zero;
        }

        return _mainSpawnPoints.HiderSpawnPoints[index].transform.position;
    }
    public Vector3 GetSeekerPosition(int index)
    {
        if (_mainSpawnPoints.SeekerSpawnPoints.Length <= index)
        {
            Debug.LogError("Seeker 스폰포인트 위치가 더작음");
            return Vector3.zero;
        }

        return _mainSpawnPoints.SeekerSpawnPoints[index].transform.position;

    }
}
