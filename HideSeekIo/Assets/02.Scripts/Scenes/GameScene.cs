using System.Collections;

using Photon.Pun;

using UnityEngine;

public class GameScene : BaseScene
{
    Transform _cameraView;

    public Transform CameraView => _cameraView;

    protected override void Init()
    {
        base.Init();
        _cameraView = this.transform.Find("cameraView");

    }

    protected virtual void Start()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            print("방장");
            PhotonGameManager.Instacne.ChangeRoomStateToServer(Define.GameState.Wait);
        }
        else
        {
            print("방장아님");
            PhotonGameManager.Instacne.State = (Define.GameState)PhotonNetwork.CurrentRoom.CustomProperties["gs"];
        }


        PhotonGameManager.Instacne.SendChattingMessageOnLocal(Define.ChattingColor.System, $"[{PhotonNetwork.CurrentRoom.Name}{DynamicTextData.gamaSceneNotice}");
    }
    public override void Clear()
    {
    }
}
