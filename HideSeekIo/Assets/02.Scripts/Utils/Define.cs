using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public delegate InGameItemUIState GameItemStateCallBack();

    public static readonly int MaxItemInventory = 3;

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
        Load,
        Null
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

    public enum PhotonObject
    {

    }
    public enum LocalWorldObject
    {
    }
    public enum Weapon
    {
        Melee2 = 1,

        Stone = 101,    //투척류
        TNT,
        Grenade,
        Flash,
        Dynamite,
        Glue,
        PoisonBomb,
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

    public enum RoomItem
    {
        HiderRandomBox,
        SeekerRandomBox,
        TNT,
    }

    public enum TimerItem
    {
        TNT
    }

	
    public enum Layer
    {
        UI = 5,
        Seeker = 8,
        Hider,
        Wall,
        SeekerItem,
        HiderItem,
        SeekerCollider,
        HiderCollider,
        Ground,
        Spike,
        AIHiderItem = 18,
        AISeekerItem = 19
            
    }

    public enum Scene
    {
        Unknown,
        Login,
        Lobby = 10,
        Main1 = 21,
        Main2 ,
        Main3 ,

        Gun1 = 51
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

    public enum InGameItem
    {
        Null,
        TNT = 10,
        Grenade,
        Glue,
        Trap,
        Vaccine,
        Speed,
        Shoes,
        //Shield,
        Stealth ,
        Dynamite = 100,
        Flash,
        PoisonBomb,
        Speed2,
        SightUp,
        Immune
    }
    public enum HiderStoreList
    {
        Trap,
        TNT,
        Flash,
        Grenade,
        Vaccine,
        Speed,
        Shoes,
        Shield,
        Stealth,
        Glue
    }

    public enum SeekrStoreList
    {
        Dynamite,
        Flash,
        PosionBomb,
        Speed2,
        SightUp,//시야증가
        Immune,
        Fire,   //시야공유용 파이어 던지기가능.
        Soil,   //끈끈이
        Mask,   //마스크, 방해효과 무시
        Rader,
        PowerUp,    //대미지증가
        Curse1,    //저주1    시야감소
        Curse2,    //저주2    방향전환

        //AI추가,팀끼리 시야공유 등

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
        GrenadeEffect,
        Ripple,
        AcidExp,
        DarkExp,
        FlashEffect
            
    }

    public enum BuffType
    {
        Null,
        B_Direction,
        B_Stun,
        B_Shoes,
        B_Speed,
        B_Shield,
        B_Sight,
        B_Revive,
        B_Stealth,
        B_SightUp,
        B_Immune
            
    }

    public enum EffectEventType
    {
        All,
        Seeker,
        Hider
    }
    public enum PhotonOnEventCode
    {
         AbilityCode = 111, 
         Projectile
    }

    public enum ChattingColor
    {
        System,
        Message
    }
    //public enum AttackSta
}
