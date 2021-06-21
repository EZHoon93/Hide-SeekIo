using System.Collections;

using UnityEngine;
using Photon.Pun;
public class HiderHealth : LivingEntity
{
    [SerializeField] GameObject _cageObject;
    Animator _animator;

    private void Awake()
    {
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        _animator = GetComponentInChildren<Animator>();
        _cageObject.SetActive(false);
    }

    [PunRPC]
    public override void Die()
    {
        base.Die();
        _cageObject.SetActive(true);
        _animator.SetTrigger("Die");
        EffectManager.Instance.EffectOnLocal(Define.EffectType.Death, this.transform.position, 0 );
        
        this.gameObject.SetLayerRecursively((int)Define.Layer.Seeker);

    }



}
