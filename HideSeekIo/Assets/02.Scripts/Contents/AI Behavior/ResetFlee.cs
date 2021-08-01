using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskCategory("EZ")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}FleeIcon.png")]
    public class ResetFlee : Action
    {
        public SharedFloat lookAheadDistance = 5;
        public SharedFloat angle;

        public SharedGameObject target;
        NavMeshAgent agent;


        public override void OnAwake()
        {
            agent = GetComponent<NavMeshAgent>();
        }
      

        // Flee from the target. Return success once the agent has fleed the target by moving far enough away from it
        // Return running if the agent is still fleeing
        public override TaskStatus OnUpdate()
        {
            if(agent == null || target.Value == null)
            {
                return TaskStatus.Failure;
            }

            agent.SetDestination(Target());

            return TaskStatus.Success;
        }


        // Flee in the opposite direction
        private Vector3 Target()
        {
            var direction = (transform.position - target.Value.transform.position).normalized;
            var newDirection = Quaternion.Euler(0, angle.Value, 0) * direction;
            return transform.position + newDirection * lookAheadDistance.Value;
        }



        // Reset the public variables
        public override void OnReset()
        {
           
        }
    }
}