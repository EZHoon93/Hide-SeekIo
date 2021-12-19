using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Dash : Skill_Base
{
    public override Define.Skill skillType { get => Define.Skill.Dash; set => base.skillType = value; }
    protected override void SetupData()
    {
        inputControllerObject.InitCoolTime = 5;
    }
    protected override void SetupCallBack()
    {
        base.SetupCallBack();
        inputControllerObject.inputType = InputType.Sub1;
    }
    public override void OnPhotonInstantiate(PlayerController playerController)
    {
        inputControllerObject.inputType = playerController.Team == Define.Team.Hide ? InputType.Sub1 : InputType.Sub2;
        //Managers.InputItemManager.SetupSkill(playerController, this);
        inputControllerObject.OnPhotonInstantiate(playerController);
    }

    public override void Use(Vector2 inputVector2)
    {
        print("스킬사용!! 대쉬");
        //playerController.character_Base.characterAvater.animator.SetTrigger("Dash");
        var AttackDirection = playerController.playerInput.GetVector2(InputType.Move);
        playerController.playerShooter.photonView.RPC("Dash", Photon.Pun.RpcTarget.AllViaServer, playerController.transform.position, playerController.playerCharacter.characterAvater.transform.forward);
    }

}
