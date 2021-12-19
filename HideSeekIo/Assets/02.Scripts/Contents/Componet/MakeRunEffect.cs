using System.Collections;

using Photon.Pun;

using UnityEngine;

public class MakeRunEffect : MonoBehaviour
{
    IMakeRunEffect _makeRunEffect;
    float lastTime;
    [SerializeField] float timeBiet;

    private void Awake()
    {
        _makeRunEffect = this.GetComponentInParent<IMakeRunEffect>();
        GetComponentInParent<IOnPhotonInstantiate>().onPhotonInstantiateEvent += OnPhotonInstantiate;
        GetComponentInParent<LivingEntity>().onDeath += () => this.gameObject.SetActive(false);
    }

    void OnPhotonInstantiate(PhotonView photonView)
    {
        
        if (_makeRunEffect.IsLocal())
        {
            this.gameObject.SetActive(true);
        }

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
        //Photon.r
        Managers.effectManager.EffectToServer(Define.EffectType.Dust, this.transform.position, 0);

        if (Managers.cameraManager.cameraTagerPlayer == null)
        {
            Managers.effectManager.EffectToServer(Define.EffectType.Dust, this.transform.position, 0);
        }
        else if(Managers.cameraManager.cameraTagerPlayer.Team == Define.Team.Hide)
        {
        }
        else
        {


            Managers.effectManager.EffectToServer(Define.EffectType.Dust, this.transform.position, 0);
        }
    }

    [PunRPC]
    void CreateEffectOnLocal()
    {

    }


}
