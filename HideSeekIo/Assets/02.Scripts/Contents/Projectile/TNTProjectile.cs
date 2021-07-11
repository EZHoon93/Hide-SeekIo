
using Photon.Pun;
using UnityEngine;

public class TNTProjectile : ThrowProjectileObject
{
    [SerializeField] ParticleSystem _effect;
    protected override void Explosion()
    {
        var usePlayer =  Managers.Game.GetLivingEntity(_useViewID);
        if (usePlayer)
        {
            PhotonNetwork.InstantiateRoomObject("TimerItem/T_TNT", this.transform.position, Quaternion.identity, 0,
            new object[] { _useViewID, 10.0f });
        }
        Push();
    }
}
