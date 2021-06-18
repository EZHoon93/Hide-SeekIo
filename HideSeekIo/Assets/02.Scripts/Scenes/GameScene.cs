using System.Collections;
using System.Collections.Generic;

using Photon.Pun;

using UnityEngine;

public class GameScene : BaseScene
{
    [SerializeField]
    Transform[] _hiderPoints;
    [SerializeField]
    Transform[] _seekerPoints;


    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Game;
        Managers.UI.ShowSceneUI<UI_Main>(); //메인 UI온 
        GameManager.Instance.CurrentGameScene = this;

        //object[] datas = { "ss", PlayerInfo.CurrentAvater };
        //PhotonNetwork.Instantiate("UserSeekr", Vector3.zero, Quaternion.identity,0,datas);
        //Managers.UI.ShowPopupUI<UI_GameLobby>();
        //Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        //gameObject.GetOrAddComponent<CursorController>();

        //GameObject player = Managers.Game.Spawn(Define.WorldObject.Player, "UnityChan");
        //Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(player);
        //Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");
        //GameObject go = new GameObject { name = "SpawningPool" };
        //SpawningPool pool = go.GetOrAddComponent<SpawningPool>();
        //pool.SetKeepMonsterCount(2);

        print(PhotonNetwork.SerializationRate + "/" + PhotonNetwork.SendRate);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.SerializationRate = 20;
            PhotonNetwork.SendRate = 40;
        }

        print(PhotonNetwork.SerializationRate + "/" + PhotonNetwork.SendRate);
    }

    public override void Clear()
    {
        
    }

    public Vector3 GetHiderPosition(int index)
    {
        if(_hiderPoints.Length < index)
        {
            Debug.LogError("Hider 스폰포인트 위치가 더작음");
            return Vector3.zero;
        }

        return _hiderPoints[index].position;
    }
    public Vector3 GetSeekrPosition(int index)
    {
        if (_seekerPoints.Length < index)
        {
            Debug.LogError("Seeker 스폰포인트 위치가 더작음");
            return Vector3.zero;
        }

        return _seekerPoints[index].position;
    }
}
