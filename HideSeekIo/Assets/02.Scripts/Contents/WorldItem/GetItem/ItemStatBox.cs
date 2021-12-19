using System;
using UnityEngine;

public class ItemStatBox : ItemBox_Base
{
    Define.StatType[] seekerStatArray =
    {
        Define.StatType.CoolTime,Define.StatType.Stealth,Define.StatType.EnergyMax,
        Define.StatType.EnergyRegen, Define.StatType.Invinc,Define.StatType.Sight,
        Define.StatType.Speed,Define.StatType.Stealth


    };
    Define.StatType[] hiderStatArray =
    {
        Define.StatType.CoolTime,Define.StatType.Stealth,Define.StatType.EnergyMax,
        Define.StatType.EnergyRegen, Define.StatType.Invinc,Define.StatType.Sight,
        Define.StatType.Speed,Define.StatType.Stealth
    };

    public override void Get(GameObject getObject)
    {
        var playerController = getObject.GetComponent<PlayerController>();
        if (playerController == null) return;
        var playerCurrentSkill = playerController.playerShooter.currentSkill;
        if (playerCurrentSkill == null) return;
        Define.StatType[] statTypes;
        if(playerController.Team == Define.Team.Hide)
        {
            statTypes = hiderStatArray;
        }
        else
        {
            statTypes = seekerStatArray;
        }
        //var selectStatArray = Managers.StatSelectManager.GetStatArrayExceptSkill(playerCurrentSkill.skillType, statTypes);

        if (playerController.IsMyCharacter())
        {
            var uimain = Managers.UI.SceneUI as UI_Main;
            //uimain.StatController.ShowSelectList(selectStatArray);
        }
    }

  
}
