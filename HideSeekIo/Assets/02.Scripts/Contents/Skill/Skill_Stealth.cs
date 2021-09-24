using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Stealth : Skill_Base
{

    public override Define.Skill skillType => Define.Skill.Staeth;
    public override Define.AttackType attakType => Define.AttackType.Button;

    protected override void SetupData()
    {
        inputControllerObject.InitCoolTime = 15;
    }

    public override void Use(Vector2 inputVector2)
    {
        print(playerController.playerHealth + "!!");
        BuffManager.Instance.CheckBuffController(playerController.playerHealth, Define.BuffType.B_Stealth);

    }
}
