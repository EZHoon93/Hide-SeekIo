using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public delegate InGameItemUIState GameItemStateCallBack();

    public enum ServerState
    {
        DisConnect,
        Connecting,
        Connect
    }
    public enum UserDataState
    {
        UnLoad,
        Wait,
        Load
    }

    public enum GameDataState
    {
        UnLoad,
        Wait,
        Load
    }

    public enum Team
    {
        Seek,
        Hide,
    }
    public enum LocalWorldObject
    {
       
    }
    public enum PhotonObject
    {
        UserSeeker,
        UserHider,
        AISeekr,
        AIHider
    }

	
    public enum Layer
    {
        Seeker = 6,
        Hider,
        Wall,
        Item
    }

    public enum Scene
    {
        Unknown,
        Login,
        Lobby = 10,
        Game,
    }
    public enum GameScene
    {
        Lobby,
        Game
    }

    public enum GameState
    {
        Wait,
        CountDown,
        GameReady,
        Gameing,
        End
    }
    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum MouseEvent
    {
        Press,
        PointerDown,
        PointerUp,
        Click,
    }

    public enum CameraMode
    {
        QuarterView,
    }

    public enum MoveHearState
    {
        Effect,
        NoEffect
    }

    public enum HiderStoreList
    {
        Speed
    }

    public enum SeekrStoreList
    {
        ChangeWeapon    ,   //무기 바꾸기
        SightCurse, //시야 감소
        DirectionCurse,  // 방향 전환
        SpeedUp,
        DamageUp,
        Immune, // 방해효과 면역
        WeaponUpgrade,
    }

    public enum InGameItemUIState
    {
        Failed,
        SucessRecycle,
        Sucess
    }

    public enum EffectType
    {
        Curse,
        BodySlam,
        Hit,
        Dust
    }

    public enum BuffType
    {
        Null,
        Direction,
        Stun
    }

    public enum PhotonOnEvent
    {
         AbilityCode = 111
    }
}
