using System.Collections;
using System.IO.Pipes;

using Photon.Pun;

using UnityEngine;

public class SeekerMove : MoveBase
{
    SeekerInput _seekerInput;

    protected override void Awake()
    {
        base.Awake();
        _seekerInput = GetComponent<SeekerInput>();
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
    }
    protected void FixedUpdate()
    {
        if (photonView.IsMine == false) return;
        OnUpdate(_seekerInput.MoveVector ,true  );
    }


 


}
