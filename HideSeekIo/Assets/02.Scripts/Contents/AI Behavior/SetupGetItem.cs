
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity
{

    [TaskCategory("EZ")]
    public class SetupGetItem : Action
    {

        public SharedGameObject seeObject;
        public SharedGetItem storeGetItem;

        public override void OnAwake()
        {

        }
        public override TaskStatus OnUpdate()
        {
            var newGetItem = seeObject.Value.GetComponent<GetWorldItemController>();
            if(newGetItem == null)
            {
                return TaskStatus.Failure;
            }

            storeGetItem.Value = newGetItem;

            return TaskStatus.Success;

        }
    }

}