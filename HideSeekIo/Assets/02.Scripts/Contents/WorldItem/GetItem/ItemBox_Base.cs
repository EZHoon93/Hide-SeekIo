using System.Collections;
using UnityEngine;
using Photon.Pun;
public abstract class ItemBox_Base : MonoBehaviourPun, IGetWorldItem
{

    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {

    }
    public virtual void OnPreNetDestroy(PhotonView rootView)
    {
     
    }
    public abstract void Get(GameObject getObject);


}
