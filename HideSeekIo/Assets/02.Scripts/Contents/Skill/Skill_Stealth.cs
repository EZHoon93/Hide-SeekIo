using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Stealth : Skill_Base
{

    protected override void SetupData()
    {
        inputControllerObject.InitCoolTime = 5;
    }

    public override void Use(Vector2 inputVector2)
    {
        BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.B_Stealth, playerController.playerHealth);
    }
}
