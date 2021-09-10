using System.Collections;
using UnityEngine;
using Photon.Pun;
public abstract class ItemBox_Base : MonoBehaviourPun, IGetWorldItem
{

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {

    }
    public void OnPreNetDestroy(PhotonView rootView)
    {
     
    }
    public abstract void Get(GameObject getObject);


}
