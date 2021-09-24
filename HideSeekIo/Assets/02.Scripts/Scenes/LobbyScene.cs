using System.Collections;
using UnityEngine;

public class LobbyScene : BaseScene
{
    public override void Clear()
    {

    }

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Lobby;
        if(Managers.UI.SceneUI== null)
        {
            Managers.UI.ShowSceneUI<UI_Main>(); //메인 UI온 
        }
        else
        {
        }

    }

}
