using System.Collections;

using DG.Tweening;

using FoW;

using UnityEngine;

public class ThrowProjectileObject : Poolable 
{
    [SerializeField] protected Transform _modelObject;
    [SerializeField] protected float _range = 2;

    protected HideInFog _hideInFog;
    protected FogOfWarUnit _fogOfWarUnit;
    protected PlayerController _attackPlayer;
    protected int _attackViewID => _attackPlayer.ViewID();
    protected int _damage;
    protected LayerMask applyLayer => 1 << (int)Define.Layer.Seeker;

    [SerializeField] AudioClip _attackClip;

    private void Reset()
    {
        if (_modelObject == null)
        {
            _modelObject = transform.MyFindChild("Model");
        }
    }
    protected virtual void Awake()
    {
        _hideInFog = GetComponent<HideInFog>();
        _fogOfWarUnit = GetComponent<FogOfWarUnit>();
        if(_modelObject == null)
        {
            _modelObject = transform.MyFindChild("Model");
        }
    }

    /// <summary>
    /// 포그오브워 설정 , 
    /// </summary>
    /// <param name="newAttackPlayer"></param>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    public virtual void Play(PlayerController attackPlayer, Vector3 startPoint, Vector3 endPoint)
    {
        if (attackPlayer == null) return;

        _attackPlayer = attackPlayer;
        _modelObject.gameObject.SetActive(true);
        _fogOfWarUnit.team = _attackViewID;
        _fogOfWarUnit.enabled = false;
        if (_attackPlayer.IsMyCharacter())
        {
            _hideInFog.SetActiveRender(true);
            _hideInFog.enabled = false;
        }
        else
        {
            _hideInFog.enabled = true;
        }
        //_hideInFog.enabled = false;
        StartCoroutine(Move(startPoint,endPoint, .0f, .2f, 2.0f ));
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

        //if (Managers.cameraManager.IsView(this.transform.position))
        //{
        //    Managers.Sound.Play(_attackClip, Define.Sound.Effect);
        //}
        if (_attackPlayer.IsMyCharacter())
        {
            Managers.cameraManager.ShakeCameraByPosition(endPoint, 0.3f, 0.5f, 0.1f);
        }
        Explosion();
    }
    
    //목표 도착시 발생
    protected virtual void Explosion()
    {

    }

  


}
