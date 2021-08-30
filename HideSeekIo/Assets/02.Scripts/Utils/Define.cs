using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public delegate InGameItemUIState GameItemStateCallBack();

    public static readonly int MaxItemInventory = 3;

    public enum StatType
    {
        Speed,
        EnergyMax,
        EnergyRegen,
        CoolTime,
        Sight
    }
    public enum SkinType
    {
        Skin,
        Weapon,
        Hat,
        Bag
    }
    public enum ProductType
    {
        Bear,
        Bunny,
        Cat,
        Weapon,
        Hat,
        Bag,
    }
    public enum ControllerType
    {
        Button,
        Joystick
    }
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
        Sniper = 201,
        Gun    
    }

    public enum ThrowItem
    {
        Stone = 100,
        TNT,
        Grenade,
        Flash,
        Dynamite,
        Glue,
        PoisonBomb,
        Null
    }

    public enum UseType
    {
        Item,
        Weapon
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
        Loading,
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
        Immune,
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
        FlashEffect,
        FogSight,
        Null
            
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

    public enum Skill
    {
        Invinc  ,   //무적
        Staeth,     //은신
        Track,      //추적
        Dash,       //대쉬
        Jump,       //점프
        Banana,      //바나나
        Flash,      //섬광탄
        Food,        //식
            Null
    }

    public enum CharacterType
    {
        Bear,
        Bunny,
        Cat,
        //Frog,
        //Dog,
        //Monkey
    }

    public enum AttackType
    {
        Button,
        Joystick
    }
  
}
