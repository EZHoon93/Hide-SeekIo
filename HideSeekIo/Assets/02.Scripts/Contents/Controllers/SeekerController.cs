using System.Collections;

using UnityEngine;

public class SeekerController : PlayerController
{
    public SeekerInput seekerInput { get; private set; }
    public SeekerMove seekerMove { get; private set; }
    public SeekerAttack seekerAttack { get; private set; }

    public SeekerHealth seekerHealth { get; private set; }


    protected void Awake()
    {
        print("Awake seeker        ;" + seekerHealth);

        seekerInput = GetComponent<SeekerInput>();
        seekerMove = GetComponent<SeekerMove>();
        seekerAttack = GetComponent<SeekerAttack>();
        seekerHealth = GetComponent<SeekerHealth>();
        TimeCoinAmount = 2;
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        seekerHealth.OnPhotonInstantiate();
        seekerInput.OnPhotonInstantiate();
        seekerMove.OnPhotonInstantiate();
        seekerAttack.OnPhotonInstantiate();
        SetActiveComponent(false);
        Util.CallBackFunction(this, 3.0f, () => SetActiveComponent(true));
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

    public override LivingEntity GetLivingEntity() => seekerHealth;
    
}
