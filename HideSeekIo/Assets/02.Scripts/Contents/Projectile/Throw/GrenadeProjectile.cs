using System.Collections;

using UnityEngine;

public class GrenadeProjectile : ThrowProjectileObject
{
    int _attackLayer = (1 << (int)Define.Layer.Seeker);
    //[SerializeField] float _range = 2;


    private void Start()
    {
        _damage = 0;
    }
    protected override void Explosion()
    {
        _modelObject.gameObject.SetActive(false);
        _fogOfWarUnit.enabled = true;
        _fogOfWarUnit.team = _attackViewID;
        Managers.effectManager.EffectOnLocal(Define.EffectType.GrenadeEffect, this.transform.position, 0,_range);
        Invoke("Push", 1.0f);

        var colliderList = UtillGame.FindInRange(this.transform, _range, applyLayer);

        foreach (var c in colliderList)
        {
            var damageable = c.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.OnDamage(_attackViewID, _damage, c.transform.position);
            }
        }
    }
}
