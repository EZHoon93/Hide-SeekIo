

using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Unity
{
    [TaskCategory("EZ")]

    public class SetObejctByLiving : Action
    {
        public SharedLivingEntity targetLivingEntity;
        public SharedGameObject storeObject;

        public override TaskStatus OnUpdate()
        {
            if(targetLivingEntity.Value == null)
            {
                return TaskStatus.Failure;
            }

            storeObject.Value = targetLivingEntity.Value.gameObject;

            return TaskStatus.Success;


        }
    }
}