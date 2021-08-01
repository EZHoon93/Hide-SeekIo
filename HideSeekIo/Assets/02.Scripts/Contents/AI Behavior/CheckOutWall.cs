
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity
{

    [TaskCategory("EZ")]
    public class CheckOutWall : Action
    {

        public SharedLayerMask rayCastLayerMask;
        public SharedVector3 offset;
        public SharedFloat distance;

        public override void OnAwake()
        {

        }
        public override TaskStatus OnUpdate()
        {
            RaycastHit hit;
            var myPos = this.transform.position + offset.Value;
            var direction = this.transform.forward + offset.Value;
            if (Physics.Raycast(myPos, direction, out hit, distance.Value, rayCastLayerMask.Value))
            {
                Debug.DrawRay(myPos, hit.point, Color.red);
                return TaskStatus.Success;
            }
            else
            {
                var temp = direction * distance.Value;
                temp.y = offset.Value.y;
                Debug.DrawRay(myPos, temp, Color.green);
                return TaskStatus.Failure;
            }
            //return hitPosition;

        }
    }

}