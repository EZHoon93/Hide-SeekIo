using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskCategory("EZ")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}FleeIcon.png")]
    public class MyFlee : NavMeshMovement
    {
        public SharedFloat fleedDistance = 20;
        public SharedFloat lookAheadDistance = 5;
        public SharedFloat angle;

        public SharedGameObject target;
        private bool hasMoved;
        

        public override void OnStart()
        {
            base.OnStart();

            hasMoved = false;

            SetDestination(Target());
        }

        // Flee from the target. Return success once the agent has fleed the target by moving far enough away from it
        // Return running if the agent is still fleeing
        public override TaskStatus OnUpdate()
        {
            if (Vector3.Magnitude(transform.position - target.Value.transform.position) > fleedDistance.Value)
            {
                return TaskStatus.Success;
            }

            if (HasArrived())
            {
                if (!hasMoved)
                {
                    return TaskStatus.Failure;
                }
                if (!SetDestination(Target()))
                {
                    return TaskStatus.Failure;
                }
                hasMoved = false;
            }
            else
            {
                // If the agent is stuck the task shouldn't continue to return a status of running.
                var velocityMagnitude = Velocity().sqrMagnitude;
                if (hasMoved && velocityMagnitude <= 0f)
                {
                    return TaskStatus.Failure;
                }
                hasMoved = velocityMagnitude > 0f;
            }

            

            return TaskStatus.Running;
        }

        // Flee in the opposite direction
        private Vector3 Target()
        {
            var direction = (transform.position - target.Value.transform.position).normalized;
            var newDirection =  Quaternion.Euler(0, angle.Value, 0) * direction;
            return transform.position + newDirection * lookAheadDistance.Value;
        }

        // Return false if the position isn't valid on the NavMesh.
        protected override bool SetDestination(Vector3 destination)
        {
            if (!SamplePosition(destination))
            {
                return false;
            }
            return base.SetDestination(destination);
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();

            fleedDistance = 20;
            lookAheadDistance = 5;
            target = null;
        }
    }
}