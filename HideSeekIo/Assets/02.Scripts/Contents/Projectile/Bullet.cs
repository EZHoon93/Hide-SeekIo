using System.Collections;

using DG.Tweening;

using UnityEngine;

public class Bullet : Poolable , IEnterTrigger
{
    [SerializeField] ParticleSystem _explosionEffect;
    public void Enter(GameObject Gettingobject)
    {
        var damageable = Gettingobject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.OnDamage(2, 1, Gettingobject.transform.position);
            Expolosion();
        }

    }

    public void Setup(Vector3 endPoint)
    {
        this.transform.DOLookAt(endPoint, 0.1f);
        var distance = Vector3.Distance(endPoint, this.transform.position);
        this.transform.DOMove(endPoint, distance * 0.1f).SetEase(Ease.Linear).OnComplete( () =>
        {

            Expolosion();
        });
    }

    void Expolosion()
    {
        Managers.Pool.Push(this);
        _explosionEffect.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
         var damageable = collision.collider.GetComponent<IDamageable>();
        print(collision.gameObject.name + "맞음");
        if (damageable != null)
        {
            print(collision.gameObject.name + "대미지");
            damageable.OnDamage(1, 1, collision.gameObject.transform.position);
        }
    }
}
