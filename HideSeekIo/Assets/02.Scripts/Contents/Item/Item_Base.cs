

using System;

using Photon.Pun;

using UnityEngine;

public abstract class Item_Base : MonoBehaviourPun
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

    public virtual void OnPhotonInstantiate(PlayerController hasPlayerController)
    {

    }

    protected void Destroy()
    {
        DestroyEvent?.Invoke();
    }
    public abstract void Use(PlayerController usePlayer);
}
