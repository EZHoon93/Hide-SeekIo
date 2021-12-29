

using Photon.Pun;
public class UI_GameJoin : UI_Button
{
    
    protected override void OnClickEvent()
    {
        var userController = (UserController)PhotonNetwork.LocalPlayer.TagObject;
        if(userController == null)
        {
            //없다면 => 에러.. 생성
            return;
        }
        userController.GameEnterToServer();

    }
}
