using System.Collections;

using DG.Tweening;

using UnityEngine;

public class Bullet : Poolable , IEnterTrigger
{
    [SerializeField] ParticleSystem _explosionEffect;
    [SerializeField] GameObject _bulletObject;
    Collider _collider;

    Vector3 _endPoint;
    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    public void Enter(GameObject Gettingobject)
    {
        //var damageable = Gettingobject.GetComponent<IDamageable>();
        //if (damageable != null)
        //{
        //    print("Enter" + Gettingobject.name);
        //    damageable.OnDamage(2, 2, Gettingobject.transform.position);
        //    Expolosion();
        //}

    }

    public void Setup(Vector3 endPoint)
    {
        CancelInvoke("AfrerDestory");
        _endPoint = endPoint;
        _collider.enabled = true;
        _bulletObject.SetActive(true);
        this.transform.DOLookAt(endPoint, 0.01f);
        var distance = Vector3.Distance(endPoint, this.transform.position);

        this.transform.DOMove(endPoint, distance * 0.1f).SetEase(Ease.Linear).OnComplete(() =>
       {

           Expolosion();
       });
    }
    //private void Update()
    //{
    //    var distance = Vector3.Distance(_endPoint, this.transform.position);
    //    if(distance > 0.3f)
    //    {
    //        this.transform.position += this.transform.forward*Time.deltaTime*10;
    //    }
    //}

    void Expolosion()
    {
        print("Expolosion" + _endPoint);
        _collider.enabled = false;
        _bulletObject.SetActive(false);
        _explosionEffect.Play();
        Invoke("AfrerDestory", 3.0f);
    }

    void AfrerDestory()
    {
        print("AfrerDestory");
        Managers.Pool.Push(this);
    }
}
