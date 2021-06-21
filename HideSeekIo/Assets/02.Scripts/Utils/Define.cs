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
        Hammer = 1,

        Stone = 50,
        Grenade,

        Sniper = 100
    }
    public enum PhotonObject
    {
        UserSeeker,
        UserHider,
        AISeekr,
        AIHider,
        ItemCoin = 20,       //아이템
        Trap,
        Box
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
        ChangeWeapon    ,   //무기 바꾸기    권총,라이플,저격총,바주카포,미사일탄
        SightCurse, //시야 감소
        DirectionCurse,  // 방향 전환
        Speed,
        Damage,
        Immune, // 방해효과 면역
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
        Shield
    }

    public enum PhotonOnEventCode
    {
         AbilityCode = 111
    }
}
