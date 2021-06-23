using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System;

public class HiderController : PlayerController
{
    public HiderMove hiderMove { get; private set; }
    public HiderInput hiderInput{ get; private set; }

    public HiderAttack hiderAttack { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        hiderMove = GetComponent<HiderMove>();
        hiderInput = GetComponent<HiderInput>();
        hiderAttack = GetComponent<HiderAttack>();
        TimeCoinAmount = 1;
    }


    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        livingEntity.OnPhotonInstantiate();
        hiderMove.OnPhotonInstantiate();
        hiderInput.OnPhotonInstantiate();
        hiderAttack.OnPhotonInstantiate();
        SetActiveComponent(true);
    }

    void SetActiveComponent(bool active)
    {
        livingEntity.enabled = active;
        hiderMove.enabled = active;
        hiderInput.enabled = active;
        hiderAttack.enabled = active;
        GetComponent<CharacterController>().enabled = active;

    }


    protected override void HandleDeath()
    {
        base.HandleDeath();
        SetActiveComponent(false);
        //GameManager.Instance.HumanDie(this.photonView.ViewID);
        if (photonView.IsMine)
        {
            
        }
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

}
