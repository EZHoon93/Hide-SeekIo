using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameStateController : MonoBehaviourPun, IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy
{
    float _initRemainTime;
    float _createServerTime;
    int _remainTime;
    bool _isPlay = false;  //다음단계 넘어갔는지 안넘어갔는지
    GameState_Base _gameState;

    Define.GameState _gameStateType;

    protected bool isNextScene;


    public Define.GameState gameStateType
    {
        get => _gameStateType;
        set
        {
            Component c = GetComponent<GameState_Base>();
            if (c)
            {
                Destroy(c);
            }
            GameState_Base newGameState = null;
            switch (value)
            {
                case Define.GameState.Wait:
                    newGameState = this.gameObject.GetOrAddComponent<GameState_Wait>();
                    break;
                case Define.GameState.CountDown:
                    newGameState = this.gameObject.GetOrAddComponent<GameState_Count>();
                    break;
                case Define.GameState.GameReady:
                    newGameState = this.gameObject.GetOrAddComponent<GameState_GameReady>();
                    break;
                case Define.GameState.Gameing:
                    newGameState = this.gameObject.GetOrAddComponent<GameState_Gameing>();
                    break;
                case Define.GameState.End:
                    newGameState = this.gameObject.GetOrAddComponent<GameState_End>();
                    break;
            }
            _gameState = newGameState;
            _gameStateType = value;
            _initRemainTime = _gameState.remainTime;
            Managers.Game.gameStateController = this;
        }
    }

    public int remainTime
    {
        get => _remainTime;
        set
        {
            if (_remainTime == value) return;
            _remainTime = value;
            _gameState.OnUpdate(_remainTime);
            if (_remainTime <= 0)
            {
                TimeEnd();
            }

        }
    }

    private void Awake()
    {
        _gameState = GetComponent<GameState_Base>();
    }

    public void ChangeInitTime(float time)
    {
        _initRemainTime = time; 
    }


    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        gameStateType = (Define.GameState)info.photonView.InstantiationData[0];
        _createServerTime = (float)info.SentServerTime;
        _isPlay = true;
        _gameState.OnPhotonInstantiate(info, _createServerTime);
    }
    public void OnPreNetDestroy(PhotonView rootView)
    {
        _gameState.OnDestroy();
    }
    private void Update()
    {
        if (_createServerTime == 0) return;
        remainTime =(int)((_initRemainTime + _createServerTime) - (float)PhotonNetwork.Time);
    }

    void TimeEnd()
    {
        if (_isPlay == false) return;
        _isPlay = false;
        _gameState.OnTimeEnd();
    }

    public void NextScene(Define.GameState gameState, object whoCanWin = null) => _gameState.NextScene(gameState, whoCanWin);
   

    
}
