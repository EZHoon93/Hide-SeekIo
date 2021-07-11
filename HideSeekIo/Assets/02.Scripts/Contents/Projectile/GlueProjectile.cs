
using Photon.Pun;

using UnityEngine;

public class GlueProjectile : ThrowProjectileObject
{
    public override void Play(int useViewID, Vector3 startPoint, Vector3 endPoint)
    {
        base.Play(useViewID, startPoint, endPoint);
        _modelObject.SetActive(true);
    }
    protected override void Explosion()
    {
        var usePlayer = Managers.Game.GetLivingEntity(_useViewID);
        if (usePlayer)
        {
            PhotonNetwork.InstantiateRoomObject("TimerItem/T_Glue", this.transform.position, Quaternion.identity, 0,
            new object[] { _useViewID, 10.0f });
        }
        Push();

    }
}
