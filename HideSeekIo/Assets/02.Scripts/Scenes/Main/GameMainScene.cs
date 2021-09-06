using System.Collections;
using System.Collections.Generic;

using Photon.Pun;

using UnityEngine;
using System;

public class GameMainScene : GameScene
{
    [SerializeField] ItemSpawnManager _itemSpawnManager;
    [SerializeField] SeekerBlock[] _seekerBlock;
    public ItemSpawnManager itemSpawnManager => _itemSpawnManager;


    public Enum[] hiderItemArray { get; private set; } =
    {
        Define.Skill.Invinc , Define.Skill.Staeth, Define.Skill.Dash,
    };


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
        //PhotonGameManager.Instacne.AddListenr(Define.GameState.Gameing, () => SetActiveSeekerBlock(false, true));
        //PhotonGameManager.Instacne.AddListenr(Define.GameState.Wait,  ()=> SetActiveSeekerBlock(true, false) );

        var CurrentState = PhotonGameManager.Instacne.State;
        switch (CurrentState)
        {
            case Define.GameState.Gameing:
            case Define.GameState.End:
                
                break;
        }
    }

    public override void Clear()
    {
        
    }

    public Enum[] GetSelectList(Define.Team team)
    {
        if(team == Define.Team.Hide)
        {
            return hiderItemArray;
        }

        return null;
    }
    void SetActiveSeekerBlock(bool active , bool isEffect)
    {
        //foreach (var s in _seekerBlock)
        //{
        //    s.Explosion(active , isEffect);
        //}
    }

    

    public Vector3 GetHiderPosition(int index)
    {
        //if(mainSpawnPoints.HiderSpawnPoints.Length <= index)
        //{
        //    Debug.LogError("Hider 스폰포인트 위치가 더작음");
        //    return Vector3.zero;
        //}

        //return mainSpawnPoints.HiderSpawnPoints[index].transform.position;
        return Vector3.zero;
    }
    public Vector3 GetSeekerPosition(int index)
    {
        //if (mainSpawnPoints.SeekerSpawnPoints.Length <= index)
        //{
        //    Debug.LogError("Seeker 스폰포인트 위치가 더작음");
        //    return Vector3.zero;
        //}

        //return mainSpawnPoints.SeekerSpawnPoints[index].transform.position;
        return Vector3.zero;
    }
}
