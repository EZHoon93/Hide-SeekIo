using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Invinc : Skill_Base
{
    //public override Define.ControllerType controllerType { get; set; } = Define.ControllerType.Button;


    protected override void SetupData()
    {
        inputControllerObject.InitCoolTime = 5;
    }

    public override void Use(Vector2 inputVector2)
    {
        BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.B_Shield, inputControllerObject.playerController.playerHealth);
    }
}
