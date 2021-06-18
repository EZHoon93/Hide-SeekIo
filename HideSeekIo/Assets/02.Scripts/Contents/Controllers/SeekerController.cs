using System.Collections;

using UnityEngine;

public class SeekerController : PlayerController
{
    public SeekerHealth seekerHealth { get; private set; }
    public SeekerInput seekerInput { get; private set; }
    public SeekerMove seekerMove { get; private set; }
    public SeekerAttack seekerAttack { get; private set; }


    public override Define.Team GetTeam() => Define.Team.Seek;
    public override LivingEntity GetLivingEntity() => seekerHealth;

    void Awake()
    {
        seekerHealth = GetComponent<SeekerHealth>();
        seekerInput = GetComponent<SeekerInput>();
        seekerMove = GetComponent<SeekerMove>();
        seekerAttack = GetComponent<SeekerAttack>();

    }

    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        seekerInput.OnPhotonInstantiate();
        seekerMove.OnPhotonInstantiate();
        seekerAttack.OnPhotonInstantiate();
    }

}
