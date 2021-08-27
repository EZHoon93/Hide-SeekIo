

using System;

using Photon.Pun;

using UnityEngine;

[RequireComponent(typeof(ObtainableItem))]

public abstract class Item_Base : MonoBehaviourPun ,IPunInstantiateMagicCallback , IOnPhotonViewPreNetDestroy
{
  

    public PlayerController hasPlayerController { get; set; }
    public Action useSuceessCallBack { get; set; }
    public event Action DestroyEvent;
    public ObtainableItem obtainableItem { get; set; }
    public Define.InGameItem InGameItemType { get; set; }


    protected virtual void Awake()
    {
        obtainableItem = GetComponent<ObtainableItem>();
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var playerViewID = (int)info.photonView.InstantiationData[0];
        hasPlayerController = Managers.Game.GetLivingEntity(playerViewID).GetComponent<PlayerController>();
        //hasPlayerController.GetAttackBase().SetupImmdediateItem(this);

    }

    public void OnPreNetDestroy(PhotonView rootView)
    {
        if (hasPlayerController)
        {
            //hasPlayerController.GetAttackBase().RemoveItem(this);
        }
    }
    protected void Destroy()
    {
        DestroyEvent?.Invoke();
    }
    public virtual void Use(PlayerController usePlayer)
    {
        UsePorecess(usePlayer);
        obtainableItem.removeCallBack?.Invoke();
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    protected abstract void UsePorecess(PlayerController usePlayer);

    public abstract Enum GetEnum();

}
