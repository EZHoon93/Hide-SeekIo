
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity
{

    [TaskCategory("EZ")]
    public class LivingAlive : Conditional
    {
        public SharedLivingEntity sharedLivingEntity;
        public SharedBool sharedBool ;
        public SharedString sharedString;
    
        public override TaskStatus OnUpdate()
        {
            if(sharedLivingEntity.Value == null)
            {
                return TaskStatus.Failure;
            }
            sharedBool.Value = sharedLivingEntity.Value.Dead;
            sharedString.Value = sharedLivingEntity.Value.ViewID().ToString();
            return sharedLivingEntity.Value.Dead ? TaskStatus.Failure : TaskStatus.Success;
        }
    }

}