using System.Collections;

using UnityEngine;

public class SleepProjectile : BulletProjectile
{
    [SerializeField] float _stunTime;

    protected override void EnterPlayer(PlayerController enterPlayer, Collider collider)
    {
        if (isPlay == false) return;
        var playerHealth = enterPlayer.playerHealth;
        if (playerHealth != null)
        {
            EffectManager.Instance.EffectOnLocal(Define.EffectType.Dust, collider.transform.position, 0);
            if (playerHealth.photonView.IsMine == false) return;
            playerHealth.OnDamage(_usePlayerViewID, 0, collider.transform.position);
            BuffManager.Instance.CheckBuffController(playerHealth, Define.BuffType.B_Sleep, _stunTime);
        }
    }


}
