using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Dash : Skill_Base
{
    public override Define.ControllerType controllerType { get; set; } = Define.ControllerType.Button;
    // Start is called before the first frame update
    private void Start()
    {
        InitCoolTime = 5;
    }

    public override void Use(PlayerController usePlayerController)
    {
        usePlayerController.GetAttackBase().Dash();
    }
}
