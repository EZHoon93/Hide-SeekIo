using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Dog : Character_Base
{
    public override Define.CharacterType characterType => Define.CharacterType.Bear;

    private void Start()
    {
        MoveSpeed = 2.5f;
        MaxEnergy = 10;
        CurrentEnergy = MaxEnergy;
    }
}
