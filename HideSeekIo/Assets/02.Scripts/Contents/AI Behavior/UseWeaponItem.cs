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
        public SharedItemEnum checkItemEnum;
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



            var direction = targetObject.Value.transform.position - _playerController.transform.position;
            var inputVector = direction / _playerController.GetAttackBase().baseWeapon.AttackDistance;
            Debug.Log(inputVector);
            _playerController.GetInputBase().Call_AttackCallBackEvent(inputVector);

            return TaskStatus.Success;
        }

        void CheckItem()
        {
            foreach(var item in _playerController.GetAttackBase().itemInventory)
            {
            }
        }
    }
}
