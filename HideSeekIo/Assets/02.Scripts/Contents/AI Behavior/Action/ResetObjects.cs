

using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Unity
{
    [TaskCategory("EZ")]

    public class ResetObjects : Conditional
    {
        public SharedLivingEntity[] sharedLivingEntities;
        public SharedGameObject[] sharedGameObjects;

        public override TaskStatus OnUpdate()
        {
            foreach(var l in sharedLivingEntities)
            {
                l.Value = null;
            }
            foreach(var g in sharedGameObjects)
            {
                g.Value = null;
            }

            return TaskStatus.Success;
        }


    }
}