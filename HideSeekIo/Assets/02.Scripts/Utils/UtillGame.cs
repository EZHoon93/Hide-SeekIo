
using UnityEngine;
using UnityEngine.AI;
using Data;

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

        return Quaternion.Euler(0, -( 90  + origanlAngle ) , 0);
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

        return new Vector2(changeVector3.x,changeVector3.z);
    }
    public static Vector3 GetInputVector3_ByCamera(Vector2 vector2)
    {
        Vector3 vector3 = new Vector3(vector2.x, 0, vector2.y);
        var quaternion = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
        var changeVector3 = quaternion * vector3;
        return new Vector3(changeVector3.x, 0,changeVector3.z);
    }

    //public static Quaternion GetSmoothRotation_ByInputVector(Vector2 inputVector2)
    //{
    //    var quaternion = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
    //    var converVector3 = ConventToVector3(inputVector2);
    //    var newDirection = quaternion * converVector3;
    //    Quaternion newRotation = Quaternion.LookRotation(newDirection);

    //}

    public static Vector3 GetThrowPosion(Vector2 inputVector2,float distance, Transform pivotTransform )
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

    public static bool ThrowZoom(Vector2 inputVector2, float distance , Transform pivorTransform, Transform zoomObject)
    {
        if (inputVector2.sqrMagnitude == 0)
        {
            zoomObject.gameObject.SetActive(false);
            return false;
        }
        var pos = GetThrowPosion(inputVector2, distance, pivorTransform);
        zoomObject.position = pos;
        zoomObject.gameObject.SetActive(true);
        return true;
    }

    public static void ZoomByLinerender()
    {
        //if (inputVector.sqrMagnitude == 0)
        //{
        //    zoomLineRenderer.enabled = false;
        //    //playerUI.GetDamageUI().gameObject.SetActive(false);
        //    return;
        //}
        //var ve = this.transform.position;
        //ve.y = 4;
        //Plane playerPlane = new Plane(Vector3.up*3, ve);
        //Vector3 theArc;
        //Vector3 center;
        //Vector3 targetPoint;
        //var temp = new Vector3(inputVector.x, 0, inputVector.y);  //direct 변환을 위해 사용
        ////_attackPlayer.CenterPivot.localPosition = inputVector.normalized * 0.4f;
        //var target = this.transform.position + (temp * 5); //타겟범위
        //float hitdist; // out 값.

        ////레이캐스트
        //Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(target));
        //targetPoint = Vector3.zero;
        //if (playerPlane.Raycast(ray, out hitdist))
        //{
        //    targetPoint = ray.GetPoint(hitdist);
        //    center = (_attackPlayer.CenterPivot.position + targetPoint) * 0.5f;
        //    center.y -= 0.5f + y;
        //    RaycastHit hitInfo;
        //    if (Physics.Linecast(_attackPlayer.CenterPivot.position, targetPoint, out hitInfo, 1 << (int)Define.Layer.Wall))
        //    {
        //        targetPoint = hitInfo.point;
        //    }
        //}
        //else
        //{
        //    targetPoint = this.transform.position;
        //    center = Vector3.zero;
        //}
        ////targetPoint.y = 0;
        ////playerUI.UpdateDamageUI(targetPoint, 5);   //타겟 위치 UI 표시
        //print(targetPoint);
        //Vector3 RelCenter = _attackPlayer.CenterPivot.position - center;
        //Vector3 aimRelCenter = targetPoint - center;
        //for (float index = 0.0f, interval = -0.0417f; interval < 1.0f;)
        //{
        //    theArc = Vector3.Slerp(RelCenter, aimRelCenter, interval += 0.0417f);
        //    zoomLineRenderer.SetPosition((int)index++, theArc + center);
        //}
        //var s = targetPoint;
        //s.y = 0;
        //zoomLineRenderer.SetPosition(24, s);


        //zoomLineRenderer.enabled = true;
    }

    //public static void ThrowObject(Vector3 startPoint, Vector3 endPoint, float arriveMinTime, float addTimeByDistance, float arriveMaxTime)
    //{

    //}

    //public static void UpdateUserMoveInput(ref Vector2 moveVector)
    //{
    //    moveVector = InputManager.Instacne.MoveVector;
    //}
    public static void UpdateUserAttackInput(ref Vector2 attackVector,ref Vector2 lastattackVector , ref bool isAttack)
    {
        //if (InputManager.Instance.AttackTouch)
        //{
        //    attackVector = InputManager.Instance.AttackVector;
        //    isAttack = true;
        //    if (attackVector.sqrMagnitude == 0)
        //    {
        //        isAttack = false;
        //    }
        //}
        //else
        //{
        //    if (isAttack)
        //    {
        //        lastattackVector = attackVector;
        //        isAttack = false;
        //    }
        //    else
        //    {
        //        attackVector = Vector2.zero;
        //        lastattackVector = Vector2.zero;
        //    }
        //}
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
        if( NavMesh.SamplePosition(randomPos, out hit, distance, NavMesh.AllAreas))
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
        if (NavMesh.SamplePosition( point, out hit, 0.1f, NavMesh.AllAreas))
        {
            return true;

        }
        else
        {
            return false;
        }
    }

    public static void DamageInRange(Vector3 center , float radius ,int damage,int damagerViewID,
        LayerMask attackLayer )
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
        Debug.Log(hitCount);
        if (hitCount > 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                if (IsTargetOnSight(center, colliders[i].transform, angle, attackLayer))
                {
                    Debug.Log(colliders[i].gameObject.name + "각도안..");
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
        LayerMask attackLayer, float angle = 360)
    {
        Collider[] colliders = new Collider[10];

        var hitCount = Physics.OverlapSphereNonAlloc(center.position, radius, colliders, attackLayer);
        if (hitCount > 0)
        {
            Debug.Log($"{hitCount} 힛");
            for (int i = 0; i < hitCount; i++)
            {
                if (IsTargetOnSight(center, colliders[i].transform, angle, attackLayer))
                {
                    Debug.LogError("시야 안 " + colliders[i].name);

                }
                else
                {
                    Debug.LogError("시야 밖 " + colliders[i].name);

                }
                var livingEntity = colliders[i].gameObject.GetComponent<LivingEntity>();
                if (livingEntity != null)
                {
                    //로컬캐릭이 실행
                    if(livingEntity.photonView.IsMine )
                    {
                        BuffManager.Instance.BuffControllerCheckOnLocal(buffType, livingEntity);
                    }
                }
            }

        }
    }

    public static bool IsTargetOnSight(Transform center , Transform target, float angle , LayerMask attackLayer)
    {
        RaycastHit hit;
        Vector3 startPoint = center.position;
        Vector3 endPoint = target.position;
        LayerMask detectionLayer = (attackLayer | 1 << (int)Define.Layer.Wall );
        startPoint.y = 0.5f;
        endPoint.y = 0.5f;

        var direction = endPoint - startPoint;

        Debug.LogError(Vector3.Angle(direction, center.forward) + "각도" + target.name);

        if (Vector3.Angle(direction, center.forward) > angle* 0.5f)
        {
            return false;
        }


        if (Physics.Raycast(startPoint, direction, out hit, 11, detectionLayer))
        {
            if (hit.transform == target) return true;
        }

        return false;
    }

    public static Data.SendAllSkinInfo MakeRandomAllSkin()
    {
        Data.SendAllSkinInfo sendAllSkinInfo;
        var ranCharacterType = Util.RandomEnum<Define.CharacterType>();
        var avaterAll = Resources.LoadAll<GameObject>($"Prefabs/Character/{ranCharacterType.ToString()}");
        var selectAvater = avaterAll[Random.Range(0, avaterAll.Length-1)].name;
        sendAllSkinInfo.autoNumber = -1;
        sendAllSkinInfo.chacterType = ranCharacterType;
        sendAllSkinInfo.avaterSkinID = selectAvater;
        return sendAllSkinInfo;
    }
}
