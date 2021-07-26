
using Photon.Pun;

using UnityEngine;

public class GlueProjectile : ThrowProjectileObject
{
    public override void Play(AttackBase attackPlayer, Vector3 startPoint, Vector3 endPoint)
    {
        base.Play( attackPlayer, startPoint, endPoint);
        _modelObject.SetActive(true);
    }
    protected override void Explosion()
    {
        EffectManager.Instance.EffectOnLocal(Define.EffectType.AcidExp, this.transform.position + new Vector3(0, 0.5f, 0), 0);
        if (attackPlayer == null) return;
        if (attackPlayer.photonView.IsMine)
        {
            //AI플레이어는 ..
            if (attackPlayer.gameObject.IsValidAI())
            {
                PhotonNetwork.InstantiateRoomObject("TimerItem/T_Glue", this.transform.position, this.transform.rotation, 0,
                new object[] { attackPlayer.ViewID(), 10.0f });
            }
            else
            {
                PhotonNetwork.Instantiate("TimerItem/T_Glue", this.transform.position, this.transform.rotation, 0,
                new object[] { attackPlayer.ViewID(), 10.0f });
            }
        }

        _modelObject.SetActive(false);
        Invoke("Push", 1.0f);
    }
}
