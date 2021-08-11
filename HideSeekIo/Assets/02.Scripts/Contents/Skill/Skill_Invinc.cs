using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Invinc : Skill_Base
{
    public override Define.ControllerType controllerType { get; set; } = Define.ControllerType.Button;

    private void Start()
    {
        InitCoolTime = 5;
    }

    public override void Use(PlayerController usePlayerController)
    {
        BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.B_Shield, usePlayerController.GetLivingEntity());
    }
}
