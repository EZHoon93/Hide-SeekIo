
using UnityEngine;
using Photon.Pun;
using Photon.Realtime; // 포톤 서비스 관련 라이브러리
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;


public class PhotonGameManager : MonoBehaviourPunCallbacks, IOnEventCallback
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
    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
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


    /// <summary>
    /// 이벤트 받을시 
    /// </summary>
    /// <param name="photonEvent"></param>
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case (byte)Define.PhotonOnEventCode.AbilityCode:
                ReciveAbility_GlobalCachedEvent(photonEvent.CustomData);
                break;
        }
    }











    #region Ability Caching Event

    public void SendAbility_GlobalCachedEvent(PlayerController playerController)
    {
        //int viewId = playerController.photonView.ViewID;
        //byte keyCode = (byte)Define.EventCode.AbilityCode;
        //bool isAI = false;
        //List<int> sendData = new List<int>();   //보낼데이터
        //foreach (var v in playerController._buyAbilityList)
        //    sendData.Add((int)v);   //현재 데이터들을 갖고옴
        //sendData.Add((int)abilityType);  //새로 추가 데이터
        ////포톤으로 보낼 데이터 만든다
        //Hashtable HT = new Hashtable();
        //HT.Add("Pv", viewId);   //적용할 캐릭 뷰 아이디
        //RemoveEvent(keyCode, HT);   //현재까지의 키코드로 데이터제거 보냄
        //HT.Add("Ab", sendData.ToArray());       //int[] 형식
        //SendEvent(viewId, keyCode, isAI, HT);   //데이터 보내기
    }

    //영구적 능력치 이벤트 받았을때 (캐싱 이벤트)
    public void ReciveAbility_GlobalCachedEvent(object photonCustomData)
    {
        //var HT = (Hashtable)photonCustomData;
        //int viewID = (int)HT["Pv"];
        //int[] datas = (int[])HT["Ab"];
        //var playerController = GameManager.instance.GetLivingEntity(viewID).GetComponent<PlayerController>();
        ////해당 플레이어에게 적용
        //AbilityManager.FindAddAbilityType(playerController, datas);

    }

    #endregion

}
