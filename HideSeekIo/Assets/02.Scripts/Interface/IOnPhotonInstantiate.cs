

using System;

using Photon.Pun;

public interface IOnPhotonInstantiate
{
    event Action<PhotonView> onPhotonInstantiateEvent;
}
