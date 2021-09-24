using System.Collections;

using UnityEngine;

public class StoneProjectile : BulletProjectile
{
    protected override void EnterPlayer(PlayerController enterPlayer, Collider collider)
    {
        if (isPlay == false) return;
        var playerHealth = enterPlayer.playerHealth;
        if (playerHealth != null)
        {
            EffectManager.Instance.EffectOnLocal(Define.EffectType.Dust, collider.transform.position, 0);
            if (playerHealth.photonView.IsMine== false) return;
            playerHealth.OnDamage(1, 0, collider.transform.position);
            //BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.B_Stun, playerHealth);
        }
    }
    
  

    
}
