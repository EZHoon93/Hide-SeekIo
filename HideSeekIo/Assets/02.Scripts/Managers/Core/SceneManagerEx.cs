using System.Collections;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime; // 포톤 서비스 관련 라이브러리
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SceneManagerEx : MonoBehaviourPunCallbacks
{
    [SerializeField] SceneAsset[] _objectModeScenes;
    [SerializeField] SceneAsset[] _itemModeScenes;
    BaseScene _currentScene;
    public BaseScene currentScene
    {
        get
        {
            if(_currentScene == null)
            {
                _currentScene = FindObjectOfType<BaseScene>();
            }
            return _currentScene;
        }
        set
        {
            _currentScene = value;
        }
    }

    public GameScene currentGameScene => currentScene as GameScene;
    string _currentGameSceneName;
    public void LoadScene(Define.Scene type, string gameSceneName = null)
    {
        Managers.Clear();

        if (currentScene.SceneType == Define.Scene.Login)
        {
            SceneManager.LoadScene(GetSceneName(type));
            return;
        }
        switch (type)
        {
            case Define.Scene.Lobby:
            case Define.Scene.Loading:
                SceneManager.LoadScene(GetSceneName(type));
                break;
            case Define.Scene.ObjectMode:
            case Define.Scene.ItemMode:
            case Define.Scene.Game:
                _currentGameSceneName = gameSceneName;
                PhotonNetwork.LoadLevel(gameSceneName);
                break;
        }
    }

    public void Clear()
    {
        currentScene.Clear();
    }

    public void LeaveGameRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void LoadSceneByIndex(int index)
    {
        Define.Scene loadSceneType = (Define.Scene)Util.GetEnumByIndex<Define.Scene>(index);
        LoadScene(loadSceneType);
    }

    public void CallRandomGameScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            var gameMode = Managers.PhotonManager.gameMode;
            PhotonNetwork.CurrentRoom.SetCustomProperties(
            new Hashtable() { { "mp", Managers.Scene.GetRandomGameScene(gameMode) } }
            );
        }

    }

    public void JoinRoom()
    {
        //서버에 연결될때 작동
        if (PhotonNetwork.IsConnected)
        {
            var roomName =  Managers.PhotonManager.roomName;
            var isScret= Managers.PhotonManager.isScret;
            var gameMode = Managers.PhotonManager.gameMode;

            MakeGameRoom(gameMode, isScret, roomName);
        }
        //서버에 연결안되면 다시연결..
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    /// <summary>
    /// 게임 참가 성공시 발생 
    /// </summary>
    public override void OnJoinedRoom()
    {
        var HT = PhotonNetwork.CurrentRoom.CustomProperties;
        var  newGameSceneName =  (string)HT["mp"];
        LoadScene(Define.Scene.Game, newGameSceneName);
    }


    /// <summary>
    /// 
    /// </summary>
    public void MakeGameRoom(Define.GameMode gameMode, bool isScreet, string roomName = null)
    {

        //방이름이 없음 => 랜덤 참여
        //var gameSceneName = GetRandomGameScene(gameMode);


        //랜덤방실패시 수동참여로 다시실행
        if (string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.JoinRandomRoom(new Hashtable() { { "gm", (int)gameMode } }, 8);
        }
        //방이름있음 => 수동참여
        else
        {
            roomName = ((int)gameMode).ToString() + roomName;
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions()
            {
                IsVisible = !isScreet,
                MaxPlayers = 8,
                CustomRoomPropertiesForLobby = new string[] { "gm" },
                CustomRoomProperties = new Hashtable() { { "gm", (int)gameMode }  ,{ "mp",GetRandomGameScene(gameMode)} }
            }, TypedLobby.Default);

        }
      
    }

    #region CallBack
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        var isScret = Managers.PhotonManager.isScret;
        var gameMode = Managers.PhotonManager.gameMode;
        var roomName = Random.Range(0, 999).ToString();
        MakeGameRoom(gameMode, isScret, roomName);
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("mp"))
        {
            var newGameSceneName =(string)propertiesThatChanged["mp"];
            //PhotonNetwork.LoadLevel(_currentGameSceneName);
            LoadScene(Define.Scene.Game, newGameSceneName);
            //var sceneIndex = (int)PhotonNetwork.CurrentRoom.CustomProperties["map"];
            //Managers.Scene.LoadSceneByIndex(sceneIndex);
        }
    }

    #endregion


    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    

    /// <summary>
    /// 게임모드에 맞춰 랜덤으로 게임씬을 찾음, 현재 진행되고 있는게임이름은 안뽑음.
    /// </summary>
    public string GetRandomGameScene(Define.GameMode gameMode)
    {
        string sceneName = null;
        //현재 게임진행 하고있는 씬이있다면..
        do
        {
            switch (gameMode)
            {
                case Define.GameMode.Item:
                    sceneName = _objectModeScenes[Random.Range(0, _objectModeScenes.Length)].name;
                    break;
                case Define.GameMode.Object:
                    sceneName = _objectModeScenes[ Random.Range(0, _objectModeScenes.Length)].name;
                    break;
            }

        } while (string.Equals(sceneName, _currentGameSceneName));


        //테스트
        sceneName = _objectModeScenes[0].name;

        return sceneName;
    }
}
