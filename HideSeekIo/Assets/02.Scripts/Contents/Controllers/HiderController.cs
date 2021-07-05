﻿using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System;

public class HiderController : PlayerController
{
    public HiderMove hiderMove { get; private set; }
    public HiderInput hiderInput{ get; private set; }
    public HiderHealth hiderHealth;

    public HiderAttack hiderAttack { get; private set; }

    protected void Awake()
    {
        hiderMove = GetComponent<HiderMove>();
        hiderInput = GetComponent<HiderInput>();
        hiderAttack = GetComponent<HiderAttack>();
        hiderHealth = GetComponent<HiderHealth>();
        hiderHealth.onDeath += HandleDeath;
        hiderHealth.onReviveEvent += HandleRevive;
        TimeCoinAmount = 1;
    }


    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        hiderHealth.OnPhotonInstantiate();
        hiderMove.OnPhotonInstantiate();
        hiderInput.OnPhotonInstantiate();
        hiderAttack.OnPhotonInstantiate();

        SetActiveComponent(true);
    }

    void SetActiveComponent(bool active)
    {
        hiderHealth.enabled = active;
        hiderMove.enabled = active;
        hiderInput.enabled = active;
        hiderAttack.enabled = active;
        GetComponent<CharacterController>().enabled = active;
    }


    protected override void HandleDeath()
    {
        base.HandleDeath();
        SetActiveComponent(false);

        if (photonView.IsMine)
        {
            //PhotonGameManager.Instacne.HiderDieOnLocal(this.ViewID());
        }
    }

    protected void HandleRevive()
    {
        SetActiveComponent(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        var enterTrigger = other.gameObject.GetComponent<IEnterTrigger>();
        print(other.gameObject.name + "부디침");
        if(enterTrigger != null)
        {
            enterTrigger.Enter(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var exitTrigger = other.gameObject.GetComponent<IExitTrigger>();
        print(other.gameObject.name + "부디침");
        if (exitTrigger != null)
        {
            exitTrigger.Exit(this.gameObject);
        }
    }

    public override LivingEntity GetLivingEntity() => hiderHealth;
    
}
