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
        public SharedThrowItemEnum checkItemEnum;
        PlayerController _playerController;
        public override void OnAwake()
        {
            _playerController = GetComponent<PlayerController>();
        }
        public override TaskStatus OnUpdate()
        {
            if(targetObject.Value == null)
            {
                return TaskStatus.Failure;
            }

            int index = CheckItem(checkItemEnum.Value);
            if(index == -1)
            {
                return TaskStatus.Failure;
            }
            else
            {
                var direction =  GetInputVector2(targetObject.Value.transform.position);
                float itemDistance = 0;
                if (index == 0)
                {
                    //_playerController.GetAttackBase().itemInventory[0].getc;
                }
                else
                {

                }

            }


            //var direction = targetObject.Value.transform.position - _playerController.transform.position;
            //var inputVector = direction / _playerController.GetAttackBase().baseWeapon.AttackDistance;
            //Debug.Log(inputVector);
            //_playerController.GetInputBase().Call_AttackCallBackEvent(inputVector);

            return TaskStatus.Success;
        }

        int CheckItem(Define.ThrowItem checkThrowItem )
        {
            int i = 0;
            foreach(var item in _playerController.GetAttackBase().itemInventory)
            {
                if(item.GetType() == typeof(Define.ThrowItem))
                {
                    //if((Define.ThrowItem) item.GetEnum() == checkThrowItem)
                    //{
                    //    return i;
                    //}
                }
                i++;
            }
            return -1;
        }

        Vector2 GetInputVector2(Vector3 targetPos)
        {
            var direction = (targetPos - _playerController.transform.position).normalized;


            return new Vector2(direction.x, direction.z);
        }
    }
}
