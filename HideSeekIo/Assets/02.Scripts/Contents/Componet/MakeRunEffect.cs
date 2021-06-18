using System.Collections;

using UnityEngine;

public class MakeRunEffect : MonoBehaviour
{
    IMakeRunEffect makeHearEffect;
    float lastTime;
    [SerializeField] float timeBiet;


    private void Awake()
    {
        makeHearEffect = GetComponentInParent<IMakeRunEffect>();
    }

    private void OnEnable()
    {
        lastTime = 0;
    }
    private void Update()
    {
        if (makeHearEffect.IsLocal() == false) return;
        if (makeHearEffect.HearState == Define.MoveHearState.Effect)
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
        EffectManager.Instance.EffectToServer(Define.EffectType.Dust, this.transform.position);
    }


}
