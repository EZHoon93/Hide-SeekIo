using System.Collections;

using UnityEngine;

public class UI_Select_Stat : UI_Select_Base
{
    [SerializeField] Define.StatType _statType;

    public Define.StatType GetStatType() => _statType;

    protected override void Click()
    {
        base.Click();
        var playerController = Managers.Game.myPlayer;
        if (!playerController) return;
        Managers.StatSelectManager.PostEvent_StatDataToServer(playerController, _statType);
    }
}
