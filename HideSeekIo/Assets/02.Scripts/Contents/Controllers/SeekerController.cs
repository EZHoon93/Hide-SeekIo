using System.Collections;

using Photon.Pun;

using UnityEngine;

public class SeekerController : PlayerController
{
    public SeekerMove seekerMove { get; private set; }
    public SeekerAttack seekerAttack { get; private set; }

    public SeekerHealth seekerHealth { get; private set; }


    protected void Awake()
    {
        seekerMove = GetComponent<SeekerMove>();
        seekerAttack = GetComponent<SeekerAttack>();
        seekerHealth = GetComponent<SeekerHealth>();
    }
    public override void OnPhotonInstantiate(PhotonView photonView)
    {
        base.OnPhotonInstantiate(photonView);
        seekerHealth.OnPhotonInstantiate();
        seekerMove.OnPhotonInstantiate();
        seekerAttack.OnPhotonInstantiate();
        if (this.IsMyCharacter())
        {
        }
    }

    void SetActiveComponent(bool acitve)
    {
        seekerHealth.enabled = acitve;
        seekerMove.enabled = acitve;
        seekerAttack.enabled = acitve;
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();
        SetActiveComponent(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        var enterTrigger = other.gameObject.GetComponent<IEnterTrigger>();
        print(other.gameObject.name + "부디침");

        if (enterTrigger != null)
        {
            enterTrigger.Enter(this.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var exitTrigger = other.gameObject.GetComponent<IExitTrigger>();
        print(other.gameObject.name + "부디침 Exit");
        if (exitTrigger != null)
        {
            exitTrigger.Exit(this.gameObject);
        }
    }

    public override LivingEntity GetLivingEntity() => seekerHealth;

    public override AttackBase GetAttackBase() => seekerAttack;

  
}
