
using UnityEngine;
using Photon.Pun;
using Photon.Realtime; // 포톤 서비스 관련 라이브러리
using Hashtable = ExitGames.Client.Photon.Hashtable;
//using System.Collections;

public class PhotonGameManager : MonoBehaviourPunCallbacks
{


    #region SingleTon
    public static PhotonGameManager Instacne
    {
        get
        {
            if (_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                _instance = FindObjectOfType<PhotonGameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }
    private static PhotonGameManager _instance; // 싱글톤이 할당될 static 변수
    #endregion





    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (Instacne != this)
        {
            // 자신을 파괴
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);


    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("GS"))
        {
            var CP = PhotonNetwork.CurrentRoom.CustomProperties;
            var gameState = (Define.GameState)CP["GS"];
            GameManager.Instance.State = gameState;
        }
    }


}
