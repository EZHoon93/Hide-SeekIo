

using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Unity
{
    [TaskCategory("EZ")]

    public class LivingExist : Conditional
    {
        public SharedLivingEntity sharedLivingEntity;

        public override TaskStatus OnUpdate()
        {
            if(sharedLivingEntity.Value == null)
            {
                return TaskStatus.Failure;
            }

            return TaskStatus.Success;
        }

       
    }
}