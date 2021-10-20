
using UnityEngine;

public class PosionBombProjectile : ThrowProjectileObject
{
    float _buffTime;

    private void Start()
    {
        _buffTime = 2;
        _damage = 0;
    }
    protected override void Explosion()
    {
        _modelObject.gameObject.SetActive(false);
        _fogOfWarUnit.enabled = true;
        EffectManager.Instance.EffectOnLocal(Define.EffectType.Curse, this.transform.position, 0,_range);
        //UtillGame.BuffInRange(this.transform, _range, Define.BuffType.B_Direction, _attackViewID, applyLayer,_buffTime);
        Invoke("Push", 1.0f);

        var colliderList = UtillGame.FindInRange(this.transform, _range, applyLayer);

        foreach(var c in colliderList)
        {
            var damageable =  c.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.OnDamage(_attackViewID, _damage, c.transform.position);
            }
            var liv = c.GetComponent<LivingEntity>();
            if (liv)
            {
                BuffManager.Instance.CheckBuffController(liv, Define.BuffType.B_Direction, _buffTime);
            }
            //var buffable = c.GetComponent<IBuffable>();
            //if(buffable != null)
            //{
            //    buffable.OnApplyBuff(Define.BuffType.B_Direction, _buffTime);
            //}
        }
    }
}
