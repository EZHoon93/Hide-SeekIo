using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UI_GameMenu : UI_Independent
{


    /// <summary>
    /// GameJoin 버튼 이벤트
    /// </summary>
    public void OnClick_GameEnter()
    {
        var userController = (UserController)PhotonNetwork.LocalPlayer.TagObject;
        if (userController == null)
        {
            //없다면 => 에러.. 생성
            return;
        }

        userController.GameEnterToServer();
    }


}
