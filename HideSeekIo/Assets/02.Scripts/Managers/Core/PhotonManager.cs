
using Photon.Pun;
using Photon.Realtime; // 포톤 서비스 관련 라이브러리
using System;
using Random = UnityEngine.Random;
public class PhotonManager : MonoBehaviourPunCallbacks
{


    #region SingleTon
    public static PhotonManager instacne
    {
        get
        {
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<PhotonManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }
    private static PhotonManager m_instance; // 싱글톤이 할당될 static 변수
    #endregion



    readonly string _gameVersion = "1.0.0";
    public Define.ServerState State { get; private set; }
    public event Action<Player> enterUserList;
    public event Action<Player> leftUserList;


    bool _isScret;
    string _roomName;


    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instacne != this)
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
        PhotonNetwork.GameVersion = _gameVersion;
        State = Define.ServerState.Connecting;  //연결중
        PhotonNetwork.ConnectUsingSettings();

    }

    public void JoinRoom()
    {
        print("JoinRoom");
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
                    MaxPlayers = 8
                }, TypedLobby.Default) ;
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
    
    // (빈 방이 없어)랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("룸참가실패");
        // 최대 4명을 수용 가능한 빈방을 생성
        int ran = Random.Range(0, 999);
        PhotonNetwork.CreateRoom(ran.ToString(), new RoomOptions { MaxPlayers = 4 });
    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        print("OnJoinedRoom");
        //PhotonNetwork.LocalPlayer.NickName = PlayerInfo.nickName;
        // 모든 룸 참가자들이 Main 씬을 로드하게 함
        Managers.Scene.LoadScene(Define.Scene.Main);

    }

    //public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    //{
    //    if(propertiesThatChanged.ContainsKey("Speed"))
    //    {
    //        var CP = PhotonNetwork.CurrentRoom.CustomProperties;
    //        print(propertiesThatChanged);
    //    }

    //    if (propertiesThatChanged.ContainsKey("GS"))
    //    {

    //        var CP = PhotonNetwork.CurrentRoom.CustomProperties;
    //        var gameState = (Define.GameState)CP["GS"];
    //        //GameManager.instance.State = gameState;
    //    }

    //}

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        enterUserList?.Invoke(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        enterUserList?.Invoke(otherPlayer);
    }

    //UI에서 다른채널 찾기 클릭시
    public void ChangeChannel(string newRoomName , bool newIsScret)
    {
        _roomName = newRoomName;
        _isScret = newIsScret;
        PhotonNetwork.LeaveRoom();
    }

    //룸나갈떄 호출
    public override void OnLeftRoom()
    {
        print("OnLeftRoo");
        Managers.Scene.LoadScene(Define.Scene.Lobby);
        
    }


}
