
using Photon.Pun;
using UnityEngine;

public abstract class Character_Base : MonoBehaviour
{
    [SerializeField] GameObject avaterObject;

    private void Awake()
    {
        GetComponent<IOnPhotonInstantiate>().OnPhotonInstantiateEvent += IOnPhotonInstantiate;
    }


    void IOnPhotonInstantiate(PhotonView photonView)
    {
        SetupUI();
    }

    protected virtual void SetupUI()
    {

    }

}
