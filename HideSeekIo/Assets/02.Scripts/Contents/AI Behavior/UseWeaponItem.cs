using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BehaviorDesigner.Runtime;

using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Unity
{

    [TaskCategory("EZ")]
    public class UseWeaponItem : Action
    {
        public SharedGameObject targetObject;
        public SharedThrowItemEnum checkThrowItem;
        public SharedImmdiateItemEnum ChecksharedImmdiateItemEnum;

        public SharedInputType sharedInputType;
        public SharedControllerInputType sharedControllerInputType;

        PlayerController _playerController;
        InputType inputType;
        SharedFloat startTime;
        public SharedFloat waitTime;
        public SharedFloat attackDistance;
        Weapon_Throw _weapon_Throw;
        //float attackDistance;
        public override void OnAwake()
        {
            _playerController = this.GetComponent<PlayerController>();
        }
        public override TaskStatus OnUpdate()
        {
            if(targetObject.Value == null)
            {
                return TaskStatus.Failure;
            }

            bool isWeapon = CheckWeapon();
            if (isWeapon)
            {
                return TaskStatus.Success;
            }


           
            //var direction = targetObject.Value.transform.position - _playerController.transform.position;
            //var inputVector = direction / _playerController.GetAttackBase().baseWeapon.AttackDistance;
            //Debug.Log(inputVector);
            //_playerController.GetInputBase().Call_AttackCallBackEvent(inputVector);
            return TaskStatus.Failure;
        }

        bool CheckWeapon()
        {
            if (checkThrowItem.Value != Define.ThrowItem.Null)
            {
                return false;
            }
            var throwWeapon = CheckThrowItem(checkThrowItem.Value);
            if (throwWeapon == null)
            {
                return false;
            }
            else
            {
                //var direction = GetInputVector2(targetObject.Value.transform.position);
                //var distance = Vector3.Distance(targetObject.Value.transform.position, _playerController.transform.position);
                //var inputVector = direction.normalized / throwWeapon.AttackDistance;

                _playerController.inputBase.controllerInputDic[throwWeapon.inputType].Call(ControllerInputType.Drag, Vector3.zero);
                sharedControllerInputType.Value = ControllerInputType.Up;
                sharedInputType.Value = throwWeapon.inputType;
                attackDistance = throwWeapon.AttackDistance;
                //_playerController.GetAttackBase().itemInventory[0].getc;
                return true;
            }
        }


        Weapon_Throw CheckThrowItem(Define.ThrowItem checkThrowItem)
        {
            int i = 0;
            foreach (var item in _playerController.GetAttackBase().ItemIntentory)
            {
                if (item == null)
                {
                    i++;
                    continue;
                }

                var weapon = item.GetComponent<Weapon_Throw>();
                if (weapon)
                {

                    if (weapon.throwType == checkThrowItem)
                    {
                        attackDistance = weapon.AttackDistance;
                        return weapon;
                    }
                }
                i++;
            }
            return null;
        }

        Vector2 GetInputVector2(Vector3 targetPos)
        {
            var direction = (targetPos - _playerController.transform.position).normalized;

            Quaternion quaternion = Quaternion.Euler(0, 0, 0);
            var result = quaternion * direction;

            return new Vector2(direction.x, direction.z);
        }
    }
}
