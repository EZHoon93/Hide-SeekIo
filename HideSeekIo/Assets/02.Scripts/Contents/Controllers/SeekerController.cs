using System.Collections;

using Photon.Pun;

using UnityEngine;

public class SeekerController : PlayerController
{
    public SeekerInput seekerInput { get; private set; }
    public SeekerMove seekerMove { get; private set; }
    public SeekerAttack seekerAttack { get; private set; }

    public SeekerHealth seekerHealth { get; private set; }


    protected void Awake()
    {
        seekerInput = GetComponent<SeekerInput>();
        seekerMove = GetComponent<SeekerMove>();
        seekerAttack = GetComponent<SeekerAttack>();
        seekerHealth = GetComponent<SeekerHealth>();
    }
    public override void OnPhotonInstantiate(PhotonView photonView)
    {
        base.OnPhotonInstantiate(photonView);
        seekerHealth.OnPhotonInstantiate();
        seekerInput.OnPhotonInstantiate();
        seekerMove.OnPhotonInstantiate();
        seekerAttack.OnPhotonInstantiate();
        if (this.IsMyCharacter())
        {
        }
    }

    void SetActiveComponent(bool acitve)
    {
        seekerHealth.enabled = acitve;
        seekerInput.enabled = acitve;
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

    public override InputBase GetInputBase() => seekerInput;
  
}
