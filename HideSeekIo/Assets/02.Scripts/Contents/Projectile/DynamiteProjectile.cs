using UnityEngine;
using Photon.Pun;

public class DynamiteProjectile : ThrowProjectileObject
{
    [SerializeField] int _damage = 1;

    protected override void Explosion()
    {
        EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position + new Vector3(0, 0.5f, 0), 0);
        if (attackPlayer== null) return;
        if (attackPlayer.photonView.IsMine )
        {
            //AI플레이어는 ..
            if (attackPlayer.gameObject.IsValidAI())
            {
                PhotonNetwork.InstantiateRoomObject("TimerItem/T_Dynamite", this.transform.position, this.transform.rotation, 0,
                new object[] { attackPlayer.ViewID(), 10.0f });
            }
            else
            {
                PhotonNetwork.Instantiate("TimerItem/T_Dynamite", this.transform.position, this.transform.rotation, 0,
                new object[] { attackPlayer.ViewID(), 2.0f });
            }
        }

        _modelObject.SetActive(false);
        Push();
    }

    //IEnumerator WaitExplosion()
    //{
    //    _fogOfWarUnit.enabled = true;

    //    while (_remainTime > 0)
    //    {
    //        _slider.value = _remainTime;
    //        print(_remainTime);
    //        _remainTime -= Time.deltaTime;
    //        yield return null;
    //    }

    //    //깜박임..
    //    yield return new WaitForSeconds(0.3f);
    //    EffectManager.Instance.EffectOnLocal(Define.EffectType.GrenadeEffect, this.transform.position, 0);
    //    UtillGame.DamageInRange(this.transform, _range, _damage, _useViewID, UtillLayer.seekerToHiderAttack);
    //    _modelObject.SetActive(false);
    //    _slider.gameObject.SetActive(false);
    //    _rangeImage.gameObject.SetActive(false);
    //    _fogOfWarUnit.enabled = true;
    //    yield return new WaitForSeconds(1.0f);
    //    Push();
    //}
}
