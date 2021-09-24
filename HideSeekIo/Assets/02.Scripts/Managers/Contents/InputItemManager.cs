using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputItemManager
{

    public void SetupControllerObject(PlayerController playerController, InputControllerObject newInputControllerObject)
    {
        var playerInput = playerController.playerInput;
        var playerShooter = playerController.playerShooter;
        if (newInputControllerObject.attackType == Define.AttackType.Button)
        {
            playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Up, newInputControllerObject.inputType, null);
            playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Down, newInputControllerObject.inputType,
              (input) => { playerShooter.UseInputControllerObject(input, newInputControllerObject); }, newInputControllerObject.sprite);

            //playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Down, newInputControllerObject.inputType,
            //(input) => { playerShooter.ZoomInputConrollerObject(input, newInputControllerObject); });
            //playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Up, newInputControllerObject.inputType,
            //  (input) => { playerShooter.UseInputControllerObject(input, newInputControllerObject); }, newInputControllerObject.sprite);
        }
        else
        {
            playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Drag, newInputControllerObject.inputType,
              (input) => {playerShooter.ZoomInputConrollerObject(input, newInputControllerObject); });
            playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Up, newInputControllerObject.inputType, (input) =>playerShooter.UseInputControllerObject(input, newInputControllerObject), newInputControllerObject.sprite);
        }
    }

    public void DestroyInputControllerObject(InputControllerObject newInputControllerObject)
    {
        var inputPlayer = newInputControllerObject.playerController;
        inputPlayer.playerInput.RemoveInputEvent(newInputControllerObject.inputType);
        //if (newInputControllerObject.playerController.IsMyCharacter())
        //{

        //}
    }


    public void SetupSkill(PlayerController playerController, Skill_Base newSkill)
    {
        var playerShooter = playerController.playerShooter;
        var currentSkill = playerShooter.currentSkill;
        if(newSkill.skillType == Define.Skill.Dash)
        {
            newSkill.transform.ResetTransform(playerController.transform);
            return;
        }
        if (currentSkill)
        {
            Managers.Resource.Destroy(currentSkill.gameObject);
        }
        playerShooter.currentSkill = newSkill;
        newSkill.transform.ResetTransform(playerController.transform);
        //SetupControllerObject(playerController, newSkill.inputControllerObject);
    }

    //public void SeupRenderController(LivingEntity livingEntity , RenderController newRenderController)
    //{
    //    livingEntity.AddRenderer(newRenderController);
    //}

    public void SetupWeapon(PlayerController playerController,Weapon newWeapon)
    {
        var playerShooter = playerController.playerShooter;
        newWeapon.transform.ResetTransform(playerController.transform);
        newWeapon.inputControllerObject.useSucessStartCallBack += () => playerShooter.WeaponAttackStart(newWeapon);
        newWeapon.inputControllerObject.useSucessEndCallBack += playerShooter.AttackBaseEnd;
        playerShooter.weaponChangeCallBack += newWeapon.WeaponChange;
        switch (newWeapon.inputControllerObject.inputType)
        {
            case InputType.Sub1:    //술래!!
                var baseWeapon = playerShooter.baseWeapon;
                if (baseWeapon)
                {
                    Managers.Resource.PunDestroy(baseWeapon);
                }
                if (newWeapon.weaponType == Weapon.WeaponType.Hammer)
                {
                    playerController.ChangeTeam(Define.Team.Seek);
                }
                if (newWeapon.weaponType == Weapon.WeaponType.Gun)
                {

                }
                playerShooter.ChangeWeapon(newWeapon, true);
                break;
            case InputType.Sub3:

                break;
        }
    }



    public void SetupImmediateGameItem(PlayerController playerController, InstantItem_Base newInstantItem)
    {
        newInstantItem.transform.SetParent(playerController.transform);
        if (playerController.playerShooter.inGameItem)
        {
          //Managers.Resource.PunDestroy(inGameItem);
        }
        //_inGameItem = item_Base.gameObject;
        //SetupControllerObject(item_Base.inputControllerObject);
    }

    /// <summary>
    /// 1회용 아이p
    /// </summary>
    /// <param name="playerController"></param>
    /// <param name="consumItem"></param>
    public void SetupConsumItem(PlayerController playerController, ConsumItem newConsumItem)
    {
        var playerShooter = playerController.playerShooter;
        var hasConsumItem = playerShooter.consumItem;
        if (hasConsumItem)
        {
            Managers.Resource.PunDestroy(hasConsumItem);
        }
        playerShooter.consumItem = newConsumItem;
    }


    //public void SetupEquipmentable(PlayerController playerController,Equipmentable equipmentable)
    //{
    //    var avater = playerController.playerCharacter.characterAvater;
    //    var adaptTransform = avater.GetSkinParentTransform(equipmentable.equipSkiType);
    //    equipmentable.model.transform.ResetTransform(adaptTransform);
    //}
}
