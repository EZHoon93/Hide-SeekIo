using System.Collections;

using UnityEngine;

public class SeekerBlock : MonoBehaviour
{
    //Collider _collider;

    private void Awake()
    {
        //_collider = GetComponent<Collider>();
    }


    public void Explosion(bool isActive, bool isEffect)
    {
        if (isEffect)
        {
            EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position, 0);
        }

        this.gameObject.SetActive(isActive);
    }


}
