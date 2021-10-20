using System.Collections;

using DG.Tweening;

using UnityEngine;

public abstract class BulletProjectile : Poolable , ICanEnterTriggerPlayer
{
    [SerializeField] GameObject _bulletObject;
    [SerializeField] ParticleSystem _effectParticle;
    Collider _collider;
    protected bool isPlay;
    protected int _usePlayerViewID;
    private void Awake()
    {
        _collider = GetComponent<Collider>();
        var mainModule = _effectParticle.main;
        mainModule.playOnAwake = false;
    }
    
    public void Enter(PlayerController enterPlayer, Collider collider)
    {
        if (isPlay == false) return;
        EnterPlayer(enterPlayer, collider);
        End();
    }

    protected abstract void EnterPlayer(PlayerController enterPlayer, Collider collider);

    public void Play(int viewID , Vector3 startPoint, Vector3 endPoint)
    {
        _usePlayerViewID = viewID;
        this.transform.position = startPoint;
        isPlay = true;
        CancelInvoke("AfrerDestory");
        _collider.enabled = true;
        _bulletObject.SetActive(true);
        this.transform.DOLookAt(endPoint, 0.01f);
        var distance = Vector3.Distance(endPoint, this.transform.position);
        this.transform.DOMove(endPoint, distance * 0.1f).SetEase(Ease.Linear).OnComplete(() =>
       {
           if (isPlay)
           {
               End();
           }
       });
    }
    protected virtual void End()
    {
        _effectParticle.Play();
        isPlay = false;
        _collider.enabled = false;
        _bulletObject.SetActive(false);
        Invoke("AfrerDestory", 3.0f);
    }

    void AfrerDestory()
    {
        if (isPlay) return;
        Managers.Pool.Push(this);
    }


}
