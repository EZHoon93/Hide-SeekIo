using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Sight : Skill_Base
{
    protected override void SetupData()
    {
        inputControllerObject.InitCoolTime = 15;
    }


    public override void Use(Vector2 inputVector2)
    {
        BuffManager.Instance.CheckBuffController(playerController.playerHealth, Define.BuffType.B_OverSee);
    }
}
