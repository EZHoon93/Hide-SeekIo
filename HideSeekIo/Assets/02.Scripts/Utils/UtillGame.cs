
using UnityEngine;
using UnityEngine.AI;
using Data;
using System.Collections.Generic;

public static class UtillGame
{

    public static Vector2 ConventToVector2(Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.z);
    }

    public static Vector3 ConventToVector3(Vector2 vector2)
    {
        return new Vector3(vector2.x, 0, vector2.y);
    }

    public static float GetAngleY(Vector2 vector2)
    {
        return Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg;
    }

    //조이스틱으로 얻은 값을 월드의 각도, 조준점 이나 공격UI시사용
    public static Quaternion WorldRotationByInput(Vector2 vector2)
    {
        var origanlAngle = GetAngleY(vector2);
        //var camerAngle = Camera.main.transform.eulerAngles.y;
        return Quaternion.Euler(0, -(90 + origanlAngle), 0);
        //return Quaternion.Euler(0, 90 - origanlAngle + camerAngle, 0);
    }

    /// <summary>
    /// 카메라 각도에 따른 3d ,월드 오브젝트 방향얻기
    /// </summary>
    /// <param name="inputVector2"></param>
    /// <returns></returns>
    public static Quaternion GetWorldRotation_ByInputVector(Vector2 inputVector2)
    {
        var quaternion = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
        var converVector3 = ConventToVector3(inputVector2.normalized);
        var newDirection = quaternion * converVector3;
        Quaternion newRotation = Quaternion.LookRotation(newDirection);

        return newRotation;
    }

    public static Vector2 GetInputVector2_ByCamera(Vector2 vector2)
    {
        Vector3 vector3 = new Vector3(vector2.x, 0, vector2.y);
        var quaternion = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
        var changeVector3 = quaternion * vector3;

        return new Vector2(changeVector3.x, changeVector3.z);
    }
    public static Vector3 GetInputVector3_ByCamera(Vector2 vector2)
    {
        Vector3 vector3 = new Vector3(vector2.x, 0, vector2.y);
        var quaternion = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
        var changeVector3 = quaternion * vector3;
        return new Vector3(changeVector3.x, 0, changeVector3.z);
    }


    public static Vector3 GetThrowPosion(Vector2 inputVector2, float distance, Transform pivotTransform)
    {

        //_attackRangeUI.position = _attackPlayer.CenterPivot.position;
        //Vector3 pos = pivotTransform.transform.position + new Vector3(inputVector2.x, 0, inputVector2.y) * distance;
        //float anlge = 0;
        //var quaternion = Quaternion.Euler(0, anlge, 0);
        var converVector3 = ConventToVector3(inputVector2);
        //var newDirection = quaternion * converVector3;
        Vector3 pos = pivotTransform.transform.position + converVector3 * distance;
        pos.y = 0;

        return pos;

    }

    public static bool ThrowZoom(Vector2 inputVector2, float distance, Transform pivorTransform, Transform zoomObject,float speed)
    {
        if (inputVector2.sqrMagnitude == 0)
        {
            zoomObject.gameObject.SetActive(false);
            return false;
        }

        var pos = GetThrowPosion(inputVector2, distance, pivorTransform);
        Debug.Log(inputVector2.magnitude);
        //zoomObject.position = pivorTransform.position + (ConventToVector3( inputVector2).normalized * inputVector2.magnitude*distance);
        //float dis = Vector3.Distance(pivorTransform.position, pos );
        //zoomObject.position = Vector3.MoveTowards(zoomObject.position, pos, Time.deltaTime*speed*inputVector2.magnitude*speed);
        zoomObject.position = Vector3.Lerp(zoomObject.position, pos, Time.deltaTime* speed * inputVector2.magnitude);

        zoomObject.gameObject.SetActive(true);
        return true;
    }

    public static Vector3 GetHitZoom(Transform attackStart , Vector3 endPoint )
    {
        Plane playerPlane = new Plane(Vector3.up, 0);

        var ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(endPoint));
        Vector3 targetPoint = Vector3.zero;
        float hitDist;
        Vector3 center;
        if(playerPlane.Raycast(ray, out hitDist))
        {
            targetPoint = ray.GetPoint(hitDist);
            center = (attackStart.transform.position + targetPoint) * 0.5f;
            center.y += 1.0f;
            RaycastHit hitInfo;
            if (Physics.Linecast(attackStart.position, targetPoint, out hitInfo, 1 << (int)Define.Layer.Water))
            {
                targetPoint = hitInfo.point;
            }
        }
        else
        {
            targetPoint = attackStart.transform.position;
        }
        targetPoint.y = 0;
        return targetPoint;
    }
 
  
    public static Vector3 GetStraightHitPoint(Transform startTransform, Vector2 inputVector, float distance)
    {
        RaycastHit hit;
        Vector3 hitPosition;
        Vector3 start = startTransform.position;
        start.y = 0.5f;
        Vector3 direction = ConventToVector3(inputVector).normalized;
        if (Physics.Raycast(start, direction, out hit, distance, 1 << (int)Define.Layer.Wall))
        {
            hitPosition = hit.point;
            hitPosition.y = startTransform.transform.position.y;
        }
        else
        {
            hitPosition = start + direction * distance;
        }
        hitPosition.y = 0.5f;
        return hitPosition;
    }

    public static Vector3 GetCurveHitPoint(Plane plane,Transform startTransform, Vector2 inputVector2, float distance)
    {
        var startPoint = startTransform.position;
        var endPoint = startTransform.position + new Vector3(inputVector2.x, 0, inputVector2.y) * distance  ;

        var ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(endPoint));
        Vector3 targetPoint = Vector3.zero;
        float hitDist;

        if (plane.Raycast(ray, out hitDist))
        {
            targetPoint = ray.GetPoint(hitDist);
            RaycastHit hitInfo;
            if (Physics.Linecast(startPoint, targetPoint, out hitInfo, 1 << (int)Define.Layer.Ground))
            {
                targetPoint = hitInfo.point;
            }
        }
        else
        {
            targetPoint = endPoint;
        }
        return targetPoint;
    }


    // 네브 메시 위의 랜덤한 위치를 반환하는 메서드
    // center를 중심으로 distance 반경 안에서 랜덤한 위치를 찾는다.
    public static Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance)
    {
        // center를 중심으로 반지름이 maxDinstance인 구 안에서의 랜덤한 위치 하나를 저장
        // Random.insideUnitSphere는 반지름이 1인 구 안에서의 랜덤한 한 점을 반환하는 프로퍼티
        Vector3 randomPos = Random.insideUnitSphere * distance + center;

        // 네브 메시 샘플링의 결과 정보를 저장하는 변수
        NavMeshHit hit;

        // randomPos를 기준으로 maxDistance 반경 안에서, randomPos에 가장 가까운 네브 메시 위의 한 점을 찾음
        if (NavMesh.SamplePosition(randomPos, out hit, distance, NavMesh.AllAreas))
        {
            return hit.position;

        }
        else
        {
            return center;
        }
        // 찾은 점 반환
    }

    public static bool IsPointOnNavMesh(Vector3 point)
    {
        NavMeshHit hit;

        // randomPos를 기준으로 maxDistance 반경 안에서, randomPos에 가장 가까운 네브 메시 위의 한 점을 찾음
        if (NavMesh.SamplePosition(point, out hit, 0.1f, NavMesh.AllAreas))
        {
            return true;

        }
        else
        {
            return false;
        }
    }

    public static List<Collider> FindInRange(Transform center, float radius, LayerMask attackLayer , float angle = 360) 
    {
        Collider[] colliders = new Collider[16];
        var hitCount = Physics.OverlapSphereNonAlloc(center.position, radius, colliders, attackLayer);
        List<Collider> resultList = new List<Collider>(16);
        if (hitCount > 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                if (IsTargetOnSight(center, colliders[i].transform, angle, attackLayer))
                {
                    resultList.Add(colliders[i] );
                }
            }

        }
        return resultList;
    }
    public static void DamageInRange(Vector3 center, float radius, int damage, int damagerViewID,
        LayerMask attackLayer)
    {
        Collider[] colliders = new Collider[10];

        var hitCount = Physics.OverlapSphereNonAlloc(center, radius, colliders, attackLayer);
        Debug.Log(hitCount);
        if (hitCount > 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                var damageable = colliders[i].gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.OnDamage(damagerViewID, damage, colliders[i].transform.position);
                }
            }
        }
    }

    public static void DamageInCompareInSight(Transform center, float radius, int damage, int damagerViewID,
        LayerMask attackLayer, float angle = 360)
    {
        Collider[] colliders = new Collider[10];
        var hitCount = Physics.OverlapSphereNonAlloc(center.position, radius, colliders, attackLayer);
        if (hitCount > 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                if (IsTargetOnSight(center, colliders[i].transform, angle, attackLayer))
                {
                    var damageable = colliders[i].gameObject.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.OnDamage(damagerViewID, damage, colliders[i].transform.position);
                    }
                }
                else
                {

                }

            }

        }
    }
    public static void BuffInRange(Transform center, float radius, Define.BuffType buffType, int damagerViewID,
        LayerMask attackLayer, float durationTime, float angle = 360)
    {
        Collider[] colliders = new Collider[10];

        var hitCount = Physics.OverlapSphereNonAlloc(center.position, radius, colliders, attackLayer);
        if (hitCount > 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                if (IsTargetOnSight(center, colliders[i].transform, angle, attackLayer))
                {
                }
                else
                {

                }
                var livingEntity = colliders[i].gameObject.GetComponent<LivingEntity>();
                if (livingEntity != null)
                {
                    //로컬캐릭이 실행
                    if (livingEntity.photonView.IsMine)
                    {
                        //BuffManager.Instance.CheckBuffController(livingEntity, buffType , durationTime);

                    }
                }
            }

        }
    }

    public static bool IsTargetOnSight(Transform center, Transform target, float angle, LayerMask attackLayer)
    {
        RaycastHit hit;
        Vector3 startPoint = center.position;
        Vector3 endPoint = target.position;
        LayerMask detectionLayer = (attackLayer | 1 << (int)Define.Layer.Wall);
        startPoint.y = 0.5f;
        endPoint.y = 0.5f;

        var direction = endPoint - startPoint;


        if (Vector3.Angle(direction, center.forward) > angle * 0.5f)
        {
            return false;
        }


        if (Physics.Raycast(startPoint, direction, out hit, 11, detectionLayer))
        {
            if (hit.transform == target) return true;
        }

        return false;
    }

    //public static SendAllSkinInfo MakeRandomAIInfo()
    //{
    //    SendAllSkinInfo sendAllSkinInfo;
    //    var ranCharacterType = Util.RandomEnum<Define.CharacterType>();
    //    var avaterAll = Resources.LoadAll<GameObject>($"Prefabs/Character/{ranCharacterType.ToString()}");
    //    var selectAvater = avaterAll[Random.Range(0, avaterAll.Length - 1)].name;
    //    sendAllSkinInfo.autoNumber = -1;
    //    sendAllSkinInfo.avaterSkinID = selectAvater;
    //    sendAllSkinInfo.accessoriesSkinID= selectAvater;
    //    sendAllSkinInfo.nickName = null;
        
    //    return sendAllSkinInfo;
    //}

    /// <summary>
    /// Y값을 같게하고 방향벡터만, 정규화X
    /// </summary>
    /// <param name="target"></param>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    public static Vector3 GetDirVector3ByEndPoint(Transform target, Vector3 endPoint)
    {
        var direction = endPoint - target.position; //방향
        direction.y = target.position.y;

        return direction;
    }
    /// <summary>
    /// 해당지점 근처로부터 건널수있는 위치를 찾을때까지 반복
    /// </summary>
    public static Vector3 GetPointOnNavMeshLoop(Vector3 center)
    {
        // center를 중심으로 반지름이 maxDinstance인 구 안에서의 랜덤한 위치 하나를 저장
        // Random.insideUnitSphere는 반지름이 1인 구 안에서의 랜덤한 한 점을 반환하는 프로퍼티
        float distance = 0;
        NavMeshHit hit;
        while (true)
        {
            Vector3 randomPos = Random.insideUnitSphere * distance + center;
            if (NavMesh.SamplePosition(randomPos, out hit, distance, NavMesh.AllAreas))
            {
                return hit.position;
            }
            distance += 0.5f;
        }
    }

}
