using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Invinc : Skill_Base
{
    //public override Define.ControllerType controllerType { get; set; } = Define.ControllerType.Button;


    protected override void SetupData()
    {
        inputControllerObject.InitCoolTime = 15;
    }

    public override void Use(Vector2 inputVector2)
    {
        //playerController.playerStat.CurrentEnergy += 5;
        BuffManager.Instance.CheckBuffController(playerController.playerHealth, Define.BuffType.B_Shield);
    }
}
