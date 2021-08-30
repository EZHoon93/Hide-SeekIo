using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Dash : Skill_Base
{
    //public override Define.ControllerType controllerType { get; set; } = Define.ControllerType.Button;
    //// Start is called before the first frame update
    //private void Start()
    //{
    //    InitCoolTime = 5;
    //}

    //public override void Use(PlayerController usePlayerController)
    //{
    //    //usePlayerController.GetAttackBase().Dash();
    //}
    protected override void SetupData()
    {
        inputControllerObject.InitCoolTime = 1;
    }

    public override void Use(Vector2 inputVector2)
    {
        //playerController.character_Base.characterAvater.animator.SetTrigger("Dash");
        var AttackDirection =  playerController.playerInput.controllerInputDic[InputType.Move].inputVector2;
        playerController.playerShooter.photonView.RPC("Dash", Photon.Pun.RpcTarget.AllViaServer, playerController.transform.position, AttackDirection);
    }

    public void UseToServer()
    {

    }
}
