using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BehaviorDesigner.Runtime;

using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Unity
{

    [TaskCategory("EZ")]
    public class ItemIsGet : Action
    {
        public SharedGameObject seeItemObject;
        public override void OnAwake()
        {

        }
        public override TaskStatus OnUpdate()
        {
            if (seeItemObject.Value == null)
            {
                return TaskStatus.Failure;
            }
            var getWorldItem = seeItemObject.Value.GetComponent<GetWorldItemController>();
            if(getWorldItem == null)
            {
                return TaskStatus.Failure;
            }

            // null 이면 아이템 얻을 수있음 => sucess
            return getWorldItem.gettingLivingEntity == null ? TaskStatus.Success : TaskStatus.Failure; 

        }
    }
}
