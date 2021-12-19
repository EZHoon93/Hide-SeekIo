using Photon.Pun;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UserController : MonoBehaviourPun, IPunInstantiateMagicCallback ,IOnPhotonViewPreNetDestroy,IPunObservable
{
    PlayerController playerController;
    ObjectController objectController;

    public string userNickName;
    public int userNumber;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
    private void Awake()
    {
        
    }
    private void OnEnable()
    {
        Managers.Game.AddListenrOnGameEvent(Define.GameEvent.GameEnter, GameJoin);
        Managers.Game.AddListenrOnGameEvent(Define.GameEvent.GameExit, GameExit);
    }
    private void OnDisable()
    {
        Managers.Game.RemoveListnerOnGameEvent(Define.GameEvent.GameEnter, GameJoin);
        Managers.Game.RemoveListnerOnGameEvent(Define.GameEvent.GameExit, GameExit);
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var infoData = info.photonView.InstantiationData;
        if (infoData == null)
        {
            return;
        }
        info.Sender.TagObject = this.gameObject;

        userNumber = (int)infoData[0];
        userNickName = (string)infoData[1]; //로컬넘버


        Managers.Game.RegisterUserController(info.Sender.ActorNumber, this);

        if (photonView.IsMine)
        {
            Managers.Game.userController = this;
        }
    }

  
    public void OnPreNetDestroy(PhotonView rootView)
    {
        Managers.Game.UnRegisterLivingEntity(rootView.ControllerActorNr);

    }
    void GameJoin(object nullObject)
    {
        if (photonView.IsMine == false) return;
        var currentSkinInfo = PlayerInfo.userData.GetCurrentAvater();
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {
            { "jn", true },
            { "ch" ,0},   // 캐릭아바타스킨
            { "we" ,0},   //무기아바타스킨
            { "ac" , -1},   //악세사리스킨

        }); ;
    }

    void GameExit(object nullObject)
    {
        if (photonView.IsMine == false) return;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "jn", false } });
    }

  
}
