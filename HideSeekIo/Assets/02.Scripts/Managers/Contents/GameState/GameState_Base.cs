using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;

public abstract class GameState_Base : MonoBehaviourPun , IPunObservable
{
    bool _isNext = false;  //다음단계 넘어갔는지 안넘어갔는지
    bool _isTimeEnd = false; //타임끝낫는지.
    double n_createTime; //생성된 시간
    int _remainTime;
    protected int _initRemainTime;
    protected UI_Main uI_Main;

    public int RemainTime
    {
        protected get => _remainTime;
        set
        {
            if (_remainTime == value) return;
            _remainTime = value;
            ChangeRemainTime();

            if (_remainTime <= 0 && _isTimeEnd == false)
            {
                _isTimeEnd = true;
                EndRemainTime();
            }

        }
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(n_createTime);
        }
        else
        {
            n_createTime = (double)stream.ReceiveNext();
        }
    }


    private void OnEnable()
    {
        n_createTime = PhotonNetwork.Time;
    }
    void Test(int s)
    {

    }
    private void Awake()
    {
        uI_Main = Managers.UI.SceneUI as UI_Main;
    }
    private void Start()
    {
      
        Setup();
    }
    private void Update()
    {
        RemainTime = (_initRemainTime - (int)(PhotonNetwork.Time - n_createTime));
    }
    protected abstract void Setup();
    protected abstract void ChangeRemainTime();
    protected abstract void EndRemainTime();


    protected virtual void Master_ChangeState(Define.GameState gameState)
    {
        if (_isNext) return;  //방장이아니거나 next가 true라면
        _isNext = true;
        PhotonGameManager.Instacne.ChangeRoomStateToServer(gameState);
    }

    public void ResetGameState()
    {
        Master_ChangeState(Define.GameState.Wait);
    }


}
