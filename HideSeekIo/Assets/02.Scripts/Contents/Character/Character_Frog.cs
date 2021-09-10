using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Frog : Character_Base
{
    private void Start()
    {
        MoveSpeed = 2.5f;
        MaxEnergy = 10;
        CurrentEnergy = MaxEnergy;
    }
}
