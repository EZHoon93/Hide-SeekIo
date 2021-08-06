using System.Collections;
using System.IO.Pipes;

using Photon.Pun;

using UnityEngine;

public class SeekerMove : MoveBase
{
   
  
    protected void FixedUpdate()
    {
        if (photonView.IsMine == false) return;
        OnUpdate(_inputBase.MoveVector ,true  );
        //UpdateStepSound();

    }





}
