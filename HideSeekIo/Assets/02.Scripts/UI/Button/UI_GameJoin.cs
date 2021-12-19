

public class UI_GameJoin : UI_Button
{

    
    protected override void OnClickEvent()
    {
        Managers.Game.NotifyGameEvent(Define.GameEvent.GameEnter);
    }


}
