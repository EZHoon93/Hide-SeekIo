using System.Collections;

using UnityEngine;

public class StoneProjectile : BulletProjectile
{
    [SerializeField] float _stunTime;
    protected override void EnterPlayer(PlayerController enterPlayer, Collider collider)
    {
        if (isPlay == false) return;
        var playerHealth = enterPlayer.playerHealth;
        if (playerHealth != null)
        {
            Managers.effectManager.EffectOnLocal(Define.EffectType.Dust, collider.transform.position, 0);
            if (playerHealth.photonView.IsMine== false) return;
            playerHealth.OnDamage(_usePlayerViewID, 0, collider.transform.position);
            //BuffManager.Instance.CheckBuffController(playerHealth, Define.BuffType.B_Stun, _stunTime);
        }
    }
    
  

    
}
