using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System;

public class HiderController : PlayerController
{
    public HiderMove hiderMove { get; private set; }
    public HiderInput hiderInput{ get; private set; }

    protected override void Awake()
    {
        base.Awake();
        hiderMove = GetComponent<HiderMove>();
        hiderInput = GetComponent<HiderInput>();
        Team = Define.Team.Hide;
        TimeCoinAmount = 1;
    }


    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        livingEntity.OnPhotonInstantiate();
        hiderMove.OnPhotonInstantiate();
        hiderInput.OnPhotonInstantiate();
        SetActiveComponent(true);
    }

    void SetActiveComponent(bool acitve)
    {
        livingEntity.enabled = acitve;
        hiderMove.enabled = acitve;
        hiderInput.enabled = acitve;
        GetComponent<CharacterController>().enabled = acitve;

    }


    protected override void HandleDeath()
    {
        base.HandleDeath();
        SetActiveComponent(false);
        GameManager.Instance.HumanDie(this.photonView.ViewID);
        if (photonView.IsMine)
        {
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var enterTrigger = other.gameObject.GetComponent<IEnterTrigger>();
        if(enterTrigger != null)
        {
            enterTrigger.Enter(this.gameObject);
        }
    }


}
