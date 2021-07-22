using UnityEngine;
using Photon.Pun;

public abstract class TimerItem : MonoBehaviour
{
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info )
    {

    }
    public virtual void EndTime()
    {

    }
}
