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


        PhotonGameManager.Instacne.SendChattingMessageOnLocal(Define.ChattingColor.System, $"[{PhotonNetwork.CurrentRoom.Name}{DynamicTextData.gamaSceneNotice}");
    }
    public override void Clear()
    {
    }
}
