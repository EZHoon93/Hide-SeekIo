using System.Collections;

using UnityEngine;

public class LoadingScene : BaseScene
{

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Lobby;

    }
    public override void Clear()
    {
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3.0f);
        PhotonManager.Instance.JoinRoom();
    }
}
