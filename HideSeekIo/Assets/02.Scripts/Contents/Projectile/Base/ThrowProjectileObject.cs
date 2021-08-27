﻿using System.Collections;

using DG.Tweening;

using FoW;

using UnityEngine;

public class ThrowProjectileObject : Poolable
{
    [SerializeField] protected GameObject _modelObject;
    
    
    protected HideInFog _hideInFog;
    protected FogOfWarUnit _fogOfWarUnit;

    public PlayerShooter playerShooter { get; set; }

    protected virtual void Awake()
    {
        _hideInFog = GetComponent<HideInFog>();
        _fogOfWarUnit = GetComponent<FogOfWarUnit>();
    }

    /// <summary>
    /// 포그오브워 설정 , 
    /// </summary>
    /// <param name="newAttackPlayer"></param>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    public virtual void Play(PlayerShooter newPlayerShooter, Vector3 startPoint, Vector3 endPoint)
    {
        playerShooter = newPlayerShooter;
        _modelObject.SetActive(true);
        _fogOfWarUnit.team = playerShooter.ViewID();
        _fogOfWarUnit.enabled = false;
        if (playerShooter.IsMyCharacter())
        {
            _hideInFog.SetActiveRender(true);
            _hideInFog.enabled = false;
        }
        else
        {
            _hideInFog.enabled = true;
        }
        StartCoroutine(Move(startPoint,endPoint, .5f, .1f, 2.0f ));
    }
    IEnumerator Move(Vector3 startPoint, Vector3 endPoint, float arriveMinTime , float addTimeByDistance, float arriveMaxTime)
    {
        var distance = Vector3.Distance(startPoint, endPoint);
        this.transform.position = startPoint;
        float arriveTime = arriveMinTime + (distance * addTimeByDistance);     // 최대거리 10 *0.1f => 1초,  10프로씩 증
        arriveTime = arriveTime < arriveMaxTime ? arriveTime : arriveMaxTime;   //arriveTime은 최대값을 넘길수없음, max보다작으면 그대로
        transform.DOMoveX(endPoint.x, arriveTime).SetEase(Ease.Linear);
        transform.DOMoveZ(endPoint.z, arriveTime).SetEase(Ease.Linear);
        transform.DOMoveY(0.3f + distance * 0.6f, arriveTime * 0.5f).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(arriveTime * 0.5f);
        transform.DOMoveY(endPoint.y, arriveTime * 0.5f).SetEase(Ease.InSine);
        yield return new WaitForSeconds(arriveTime * 0.5f);
        Explosion();
    }
    

    //목표 도착시 발생
    protected virtual void Explosion()
    {

    }

  


}
