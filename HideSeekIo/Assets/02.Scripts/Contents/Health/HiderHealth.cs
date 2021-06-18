using System.Collections;

using UnityEngine;

public class HiderHealth : LivingEntity
{
    [SerializeField] GameObject _cageObject;
    public void OnPhotonInstantiate()
    {
        _cageObject.SetActive(false);
    }

    public override void Die()
    {
        base.Die();
        _cageObject.SetActive(true);
    }
}
