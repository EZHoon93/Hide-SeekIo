using System.Collections;
using System.IO.Pipes;

using Photon.Pun;

using UnityEngine;

public class SeekerMove : MoveBase
{
   
  
    protected void FixedUpdate()
    {
        if (photonView.IsMine == false) return;


        OnUpdate(_inputBase.controllerInputDic[InputType.Move].inputVector2  ,true  );
        //UpdateStepSound();
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        var move = new Vector2(h, v);
        move = UtillGame.GetInputVector2_ByCamera(move);
#if UNITY_EDITOR
        OnUpdate(move, true);

#endif

    }





}
