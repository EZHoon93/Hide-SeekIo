

using System;

using Photon.Pun;

using UnityEngine;

public abstract class Item_Base : MonoBehaviourPun , IItem,IPunInstantiateMagicCallback , IOnPhotonViewPreNetDestroy
{
    public event Action DestroyEvent;
    public enum UseState
    {
        Local,
        Server
    }

    public enum UseType
    {
        Item,
        Weapon
    }

    public UseState State { get; protected set; }
    public UseType useType { get; protected set; }

    [SerializeField] Sprite itemSprite;

    public Sprite ItemSprite => itemSprite;
    public PlayerController hasPlayerController { get; set; }
    Define.UseType IItem.useType { get; set; }

    private void Reset()
    {
        print($"Sprites/InGameItem/{this.name}" +"ㅇㅇㅇㅇ");
        itemSprite = Resources.Load<Sprite>($"Sprites/InGameItem/{this.gameObject.name}");

    }
    protected virtual void Awake()
    {
        State = UseState.Server;
        useType = UseType.Item;
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var playerViewID = (int)info.photonView.InstantiationData[0];
        hasPlayerController = Managers.Game.GetLivingEntity(playerViewID).GetComponent<PlayerController>();
        hasPlayerController.GetAttackBase().SetupImmdediateItem(this);

    }

    public void OnPreNetDestroy(PhotonView rootView)
    {
        if (hasPlayerController)
        {
            hasPlayerController.GetAttackBase().RemoveItem(this);
        }
    }
    protected void Destroy()
    {
        DestroyEvent?.Invoke();
    }
    public virtual void Use(PlayerController usePlayer)
    {
        UsePorecess(usePlayer);
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    protected abstract void UsePorecess(PlayerController usePlayer);

    public Sprite GetSprite() => itemSprite;
    

    public bool Zoom(Vector2 inputVector)
    {
        return false;
    }
    

    public void Attack(Vector2 inputVector)
    {
        
    }

    
}
