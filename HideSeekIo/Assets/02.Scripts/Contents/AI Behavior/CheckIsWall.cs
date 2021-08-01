
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity
{

    [TaskCategory("EZ")]
    public class CheckIsWall : Action
    {

        public SharedLayerMask rayCastLayerMask;
        public SharedGameObject seeObject;
        public SharedVector3 offset;
        public SharedFloat distance;

        public override void OnAwake()
        {
           
        }
        public override TaskStatus OnUpdate()
        {
            RaycastHit hit;
            var myPos = this.transform.position + offset.Value;
            var targetPos = seeObject.Value.transform.position + offset.Value;
            var direction = targetPos - myPos;
            direction.y = 0;
            if (Physics.Raycast(myPos, direction.normalized, out hit, distance.Value, rayCastLayerMask.Value))
            {
                Debug.DrawRay(myPos, direction, Color.red);
                return TaskStatus.Success;
            }
            else
            {
                Debug.DrawRay(myPos, direction, Color.red);
                return TaskStatus.Failure;
            }
            //return hitPosition;

        }
    }

}