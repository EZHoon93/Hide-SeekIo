using System.Collections;
using System.Collections.Generic;

using Photon.Pun;

using UnityEngine;

public class GameMainScene : GameScene
{
    [SerializeField] MainSpawnPoints _mainSpawnPoints;
    [SerializeField] ItemSpawnManager _itemSpawnManager;
    [SerializeField] SeekerBlock[] _seekerBlock;

    public MainSpawnPoints mainSpawnPoints => _mainSpawnPoints;
    public ItemSpawnManager itemSpawnManager => _itemSpawnManager;

    protected override void Init()
    {
        base.Init();
        Managers.UI.ShowSceneUI<UI_Main>(); //메인 UI온 
        Managers.Game.CurrentGameScene = this;
        
        Managers.Sound.Play("BGM_Main1", Define.Sound.Bgm, 1.0f);
    }

    protected override void Start()
    {
        base.Start();
        PhotonGameManager.Instacne.AddListenr(Define.GameState.Gameing, () => SetActiveSeekerBlock(false, true));
        PhotonGameManager.Instacne.AddListenr(Define.GameState.Wait,  ()=> SetActiveSeekerBlock(true, false) );

        var CurrentState = PhotonGameManager.Instacne.State;
        switch (CurrentState)
        {
            case Define.GameState.Gameing:
            case Define.GameState.End:
                foreach (var s in _seekerBlock)
                {
                    SetActiveSeekerBlock(false, false);
                }
                break;
        }
    }

    public override void Clear()
    {
        
    }

    void SetActiveSeekerBlock(bool active , bool isEffect)
    {
        foreach (var s in _seekerBlock)
        {
            s.Explosion(active , isEffect);
        }
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
