using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using UnityEngine;

public class Skill_Jump : Skill_Base
{
    UI_ControllerJoystick _controllerJoystick;
    Vector3 start;
    Vector3 end;
    Transform target;
    Vector3 targetPoint; 
    protected override void SetupData()
    {
        inputControllerObject.InitCoolTime = 3;
    }

    public override void OnPhotonInstantiate(PlayerController playerController)
    {
        base.OnPhotonInstantiate(playerController);

        _controllerJoystick = InputManager.Instance.GetControllerJoystick(inputControllerObject.inputType);
    }

    public override void Use(Vector2 inputVector2)
    {
        ////playerController.character_Base.characterAvater.animator.SetTrigger("Dash");
        //var AttackDirection = playerController.playerInput.controllerInputDic[InputType.Move].inputVector2;
        //playerController.playerShooter.photonView.RPC("Dash", Photon.Pun.RpcTarget.AllViaServer, playerController.transform.position, playerController.playerCharacter.characterAvater.transform.forward);
        playerController.transform.DOJump(targetPoint, 2, 1, 1);
    }

    private void Update()
    {
        if (playerController.IsMyCharacter() == false) return;
        var wall = GetWall();
        print(wall);
        _controllerJoystick._ultimateJoystick.enabled = wall;
        //Debug.DrawLine(start, end, Color.blue);
        Debug.DrawRay(start, end,Color.green);
    }

    bool GetWall()
    {
        RaycastHit raycastHit;
        Vector3 startPoint = playerController.transform.position;
        startPoint.y = 0.5f;
        //Vector3 direction = startPoint + playerController.playerCharacter.characterAvater.transform.forward;
        Vector3 direction = playerController.playerCharacter.characterAvater.transform.forward;
        direction.y = 0.5f;
        //Vector3 endPoint = playerController
        start = startPoint;
        end = direction;
        if (Physics.Raycast(startPoint,direction,out raycastHit, 10, 1<<(int)Define.Layer.Wall))
        {
            targetPoint = raycastHit.collider.transform.position + direction * 2;
            targetPoint.y = 0;
            return true;
        }
        return false;
    }

    //Vector3 GetWall()
    //{
    //    RaycastHit raycastHit;
    //    Vector3 startPoint = playerController.transform.position;
    //    startPoint.y = 0.5f;
    //    //Vector3 direction = startPoint + playerController.playerCharacter.characterAvater.transform.forward;
    //    Vector3 direction = playerController.playerCharacter.characterAvater.transform.forward;
    //    direction.y = 0.5f;
    //    //Vector3 endPoint = playerController
    //    start = startPoint;
    //    end = direction;
    //    if (Physics.Raycast(startPoint, direction, out raycastHit, 10, 1 << (int)Define.Layer.Wall))
    //    {
    //        return true;
    //    }
    //    return false;
    //}
}
