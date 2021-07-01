using System.Collections;

using UnityEngine;

public class MakeRunEffect : MonoBehaviour
{
    IMakeRunEffect _makeRunEffect;
    float lastTime;
    [SerializeField] float timeBiet;

    private void Awake()
    {
        _makeRunEffect =  this.GetComponentInParent<IMakeRunEffect>();
        GetComponentInParent<IOnPhotonInstantiate>().OnPhotonInstantiateEvent += OnPhotonInstantiate;
    }

    void OnPhotonInstantiate()
    {
        this.gameObject.SetActive(true);
        lastTime = 0;
    }
    private void Update()
    {
        //if (_makeRunEffect.IsLocal() == false) return;
        if (_makeRunEffect.HearState == Define.MoveHearState.Effect)
        {
            if (Time.time >= lastTime + timeBiet)
            {
                lastTime = Time.time;
                CreateEffect();
            }
        }

    }

    void CreateEffect()
    {
        EffectManager.Instance.EffectToServer(Define.EffectType.Dust, this.transform.position, 0 );
    }


}
