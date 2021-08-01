using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BehaviorDesigner.Runtime;

using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Unity
{

    [TaskCategory("EZ")]
    public class AICheckItem : Action
    {
        public SharedItemEnum  checkItemEnum;
        PlayerController _playerController;
        public override void OnAwake()
        {
            _playerController = GetComponent<PlayerController>();
        }
        public override TaskStatus OnUpdate()
        {
            var isExist = _playerController.itemInventory.Any(s => s.itemType == checkItemEnum.Value);
            if (isExist)
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }


        }
    }
}
