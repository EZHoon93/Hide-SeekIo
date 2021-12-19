using System.Collections;

using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.EZ
{
    [TaskCategory("EZ")]

    public class SeeObjects : Conditional
    {
        public SharedTeam sharedThisTeam;
        public SharedFloat checkTimeBet;
        public SharedInt rayCastCount;
        public SharedLayerMask seeLayer;
        public SharedFloat angle;
        public SharedFloat distance;
        public SharedBool debugRay;

        public SharedGameObject seeLivingObject;
        public SharedGetItem stroeItem;

        float _timeBet = 0;

        //public SharedString sharedString;
        //LivingEntity _seeLivingObject;

        public override TaskStatus OnUpdate()
        {

            seeLivingObject.Value = null;
            if (_timeBet > 0)
            {
                _timeBet -= Time.deltaTime;
                return TaskStatus.Failure;
            }
            else
            {
                _timeBet = checkTimeBet.Value;
            }

            var angleInterval = angle.Value / rayCastCount.Value;   //발사할 레이캐스트 
            var rayCastStartPoint = this.transform.position;
            rayCastStartPoint.y = 0.5f;
            Vector3 direction = this.transform.forward;
            direction.y = 0.5f;
            float minDistance = 9999;
            Vector3 hitPoint = Vector3.zero;

            for (int i = -rayCastCount.Value / 2; i < rayCastCount.Value / 2 + 1; i++)
            {
                RaycastHit hit;
                Quaternion quaternion = Quaternion.Euler(0, angleInterval * i, 0);
                var newDirection = quaternion * direction;
                newDirection.y = 0;
                if (Physics.Raycast(rayCastStartPoint, newDirection, out hit, distance.Value, seeLayer.Value))
                {
                    hitPoint = hit.point;
                    hitPoint.y = 0;
                    //Check LivingEntity
                    int seeLayerIndex = sharedThisTeam.Value == Define.Team.Seek ? (int)Define.Layer.Hider : (int)Define.Layer.Seeker;
                    if (hit.collider.gameObject.layer == seeLayerIndex)
                    {
                        var livingEntity = hit.collider.gameObject.GetComponent<LivingEntity>();
                        //if (livingEntity == null) continue;
                        //최초 라면 만약 없다면 해당값을 최소값으로 등록
                        if (livingEntity == null || livingEntity.Dead) continue;
                        else
                        {
                            if (seeLivingObject.Value == null)
                            {
                                seeLivingObject.Value = livingEntity.gameObject;
                                minDistance = Vector3.Distance(this.transform.position, livingEntity.transform.position);
                            }
                            else
                            {
                                var distance = Vector3.Distance(this.transform.position, livingEntity.transform.position);
                                if (distance < minDistance)
                                {
                                    seeLivingObject.Value = livingEntity.gameObject;
                                    minDistance = distance;
                                }
                            }
                        }
                    }
                    //
                    //if(//게임아이템)
                }
                else
                {
                    hitPoint = rayCastStartPoint + newDirection * distance.Value;
                    hitPoint.y = 0.5f;
                }

                if (debugRay.Value)
                {
#if UNITY_EDITOR

                    Debug.DrawRay(rayCastStartPoint, hitPoint, Color.green);
#endif
                }

            }   //For문 


            if(seeLivingObject.Value != null)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;

        }


    }
}