

using System;

using Photon.Pun;

using UnityEngine;

[RequireComponent(typeof(InputControllerObject))]
[RequireComponent(typeof(ConsumItem))]

public abstract class InstantItem_Base : MonoBehaviourPun ,IPunInstantiateMagicCallback , IOnPhotonViewPreNetDestroy
{
    public PlayerController hasPlayerController { get; set; }
    public InputControllerObject inputControllerObject{ get; set; }
    public virtual Define.InGameItem InGameItemType { get; set; }
    ConsumItem _consumItem;

    protected virtual void Awake()
    {
        inputControllerObject = GetComponent<InputControllerObject>();
        _consumItem = GetComponent<ConsumItem>();
        SetupCallBack();
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var playerViewID = (int)info.photonView.InstantiationData[0];
        hasPlayerController = Managers.Game.GetLivingEntity(playerViewID).GetComponent<PlayerController>();
        //Managers.InputItemManager.SetupImmediateGameItem(hasPlayerController, this);
        _consumItem.OnPhotonInstantiate(hasPlayerController);
        inputControllerObject.OnPhotonInstantiate(hasPlayerController);
    }
    protected virtual void SetupCallBack()
    {
        inputControllerObject.attackType = Define.AttackType.Button;
        inputControllerObject.inputType = InputType.Sub3;
        inputControllerObject.shooterState = PlayerShooter.state.MoveToAttackPoint;
        inputControllerObject.AddUseEvent(Use);
        inputControllerObject.AddZoomEvent(null);
        inputControllerObject.InitCoolTime = 3;
    }
    public void OnPreNetDestroy(PhotonView rootView)
    {
        if (hasPlayerController == null) return;
        inputControllerObject.OnDestroyEvent();
    }

    public virtual void UseEffect()
    {

    }
   
    public virtual void Use(Vector2 inputVector2)
    {
        if (hasPlayerController == null) return;
        //_consumItem?.Use(hasPlayerController, inputControllerObject.inputType);
        UsePorecess(hasPlayerController);
        Managers.Resource.PunDestroy(this);
    }

    protected abstract void UsePorecess(PlayerController usePlayer);
}
