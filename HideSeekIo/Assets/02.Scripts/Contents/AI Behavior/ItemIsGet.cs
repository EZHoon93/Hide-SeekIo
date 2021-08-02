using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BehaviorDesigner.Runtime;

using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Unity
{

    [TaskCategory("EZ")]
    public class ItemIsGet : Conditional
    {
        public SharedGetItem seeGetItem;
        public override void OnAwake()
        {

        }
        public override TaskStatus OnUpdate()
        {
            if (seeGetItem.Value == null)
            {
                return TaskStatus.Failure;
            }

            // null 이면 아이템 얻을 수있음 => sucess
            Debug.Log("지속ㄱ 검사");
            return seeGetItem.Value.gettingLivingEntity == null ? TaskStatus.Success : TaskStatus.Failure; 

        }
    }
}
