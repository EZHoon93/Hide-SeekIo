
using Photon.Pun;
using Photon.Realtime; // 포톤 서비스 관련 라이브러리
using System;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonManager : MonoBehaviourPunCallbacks
{


    #region SingleTon
    public static PhotonManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                _instance = FindObjectOfType<PhotonManager>();
            }

            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }
    private static PhotonManager _instance; // 싱글톤이 할당될 static 변수
    #endregion



    readonly string _gameVersion = "1.0.0";
    public Define.ServerState State { get; private set; }
    


    bool _isScret;
    string _roomName;


    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (Instance != this)
        {
            // 자신을 파괴
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    //최초 로그인 후 포톤에 접속시 
    private void Start()
    {
        _isScret = false;
        _roomName = null;
    }


    //포톤 서버 연결
    public void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = _gameVersion;
        State = Define.ServerState.Connecting;  //연결중
        PhotonNetwork.ConnectUsingSettings();

    }

    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (string.IsNullOrEmpty(_roomName))
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // 최대 4명을 수용 가능한 빈방을 생성
                int ran =  Random.Range(0, 999);
                PhotonNetwork.JoinOrCreateRoom(_roomName, new RoomOptions()
                {
                    IsVisible = _isScret,
                    MaxPlayers = 8,
                    EmptyRoomTtl = 1,
                    PlayerTtl = 1,
                }, TypedLobby.Default); ; ;
            }
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    /// 포톤 서버 접속시 자동 실행
    public override void OnConnectedToMaster()
    {
        print("OnConnectedToMaster");
        //마스터 서버에 접속중이라면
        if (PhotonNetwork.IsConnected)
        {
            // 룸 접속 실행
            State = Define.ServerState.Connect;
            
        }
        else
        {
            State = Define.ServerState.DisConnect;
            //접속 실패시 접솔 실패 UI
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    //정보 업데이트. 닉네임,레벨,참여여부등
    public void PhotonLogin()
    {
        PhotonNetwork.LocalPlayer.NickName = PlayerInfo.nickName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {
            { "jn", false } 
        });
    }
    
    // (빈 방이 없어)랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("OnJoinRandomFailed");

        //print("룸참가실패");
        // 최대 4명을 수용 가능한 빈방을 생성
        int ran = Random.Range(0, 999);
        PhotonNetwork.CreateRoom(ran.ToString(), new RoomOptions { MaxPlayers = 4 });
    }
  
    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        print("OnJoinedRoom");

        if (PhotonNetwork.IsMasterClient)
        {
            print("OnJoined Room Master");
            PhotonNetwork.LoadLevel("Main1");
            //Managers.Scene.MasterSelectNextMainScene(Define.Scene.Unknown);
        }
        else
        {
            print("OnJoined Room Message 변경 펄스");
            //var sceneIndex = (int)PhotonNetwork.CurrentRoom.CustomProperties["map"];
            //Managers.Scene.LoadSceneByIndex(sceneIndex);
        }
    }
    public override void OnConnected()
    {
        print("OnConnected!!");
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("map"))
        {
            var sceneIndex = (int)PhotonNetwork.CurrentRoom.CustomProperties["map"];
            Managers.Scene.LoadSceneByIndex(sceneIndex);
        }

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //print("마스터 바뀜!!");
    }



    //UI에서 다른채널 찾기 및 미입력시 빠른채널찾기
    public void ChangeChannel(string newRoomName= null , bool newIsScret =false)
    {
        _roomName = newRoomName;
        _isScret = newIsScret;
        PhotonNetwork.LeaveRoom();
    }

    //룸나갈떄 호출
    public override void OnLeftRoom()
    {
        //print("OnLeftRoo");
        Managers.Scene.LoadScene(Define.Scene.Lobby);
        
    }
 
}
