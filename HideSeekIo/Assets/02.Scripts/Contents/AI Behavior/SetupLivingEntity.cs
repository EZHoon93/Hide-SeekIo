
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity
{

    [TaskCategory("EZ")]
    public class SetupLivingEntity : Action
    {

        public SharedGameObject targetObject;
        public SharedLivingEntity storeLivingEntity;

        public override TaskStatus OnUpdate()
        {
            if(targetObject.Value == null)
            {
                return TaskStatus.Failure;
            }
            var livingEntity = targetObject.Value.GetComponent<LivingEntity>();
            if(livingEntity == null)
            {
                return TaskStatus.Failure;
            }
            storeLivingEntity.Value = livingEntity;
            return TaskStatus.Success;
        }
    }

}