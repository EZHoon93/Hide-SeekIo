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
    public enum Weapon
    {
        Melee2 = 1,

        Stone = 101,
        Grenade,

        Sniper = 201
    }

    public enum AIType
    {
        AISeeker ,
        AIHider
    }
    public enum WorldItem
    {
        ItemCoin ,       //중립 아이템
        Trap = 101,
        Box,
    }

	
    public enum Layer
    {
        UI = 5,
        Seeker = 8,
        Hider,
        Wall,
        Item
    }

    public enum Scene
    {
        Unknown,
        Login,
        Lobby = 10,
        Main,
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
        Trap,
        Shield,
        Shoes,
        Box,
        Grenade,
        Speed
    }

    public enum SeekrStoreList
    {
        ChangeWeapon    ,   // 수류탄 추가 
        SightCurse, //시야 감소
        DirectionCurse,  // 방향 전환
        Speed,  //이속증가
        AllumanClap,    //스턴 면역
        BodyUp, // 방해효과 면역 && 몸통박치기시 박스 파괴,
        RangeUp ,//무기 범위증가
        RaderUp ,//레이더 범위증가

        
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
        Dust,
        Death,
        Coin,
        CloudBurst,
        TrapEffect,
        BuffEffect,
        GrenadeEffect
    }

    public enum BuffType
    {
        Null,
        Direction,
        Stun,
        Shoes,
        Speed,
        Shield,
        Sight
            
    }

    public enum EffectEventType
    {
        All,
        Seeker,
        Hider
    }
    public enum PhotonOnEventCode
    {
         AbilityCode = 111
    }

    //public enum AttackSta
}
