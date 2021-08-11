using System.Collections;

using Photon.Pun;

using UnityEngine;

public class GameScene : BaseScene
{
    Transform _cameraView;
    protected int _initGameTime = 180;

    public Transform CameraView => _cameraView;
    public int InitGameTime => _initGameTime;


    public Transform test;

    protected override void Init()
    {
        base.Init();
        _cameraView = this.transform.Find("cameraView");

    }

    protected virtual void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonGameManager.Instacne.ChangeRoomStateToServer(Define.GameState.Wait);
        }
        else
        {
            var gameState = (Define.GameState)PhotonNetwork.CurrentRoom.CustomProperties["gs"];
            PhotonGameManager.Instacne.State = gameState;
        }

        if((bool)PhotonNetwork.LocalPlayer.CustomProperties["jn"])
        {
            PhotonGameManager.Instacne.GameJoin();
        }
        else
        {
            PhotonGameManager.Instacne.GameExit();

        }

        PhotonGameManager.Instacne.SendChattingMessageOnLocal(Define.ChattingColor.System, $"[{PhotonNetwork.CurrentRoom.Name}{DynamicTextData.gamaSceneNotice}");


    }
    public override void Clear()
    {
    }
}
