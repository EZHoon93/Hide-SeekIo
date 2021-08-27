using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Bear : Character_Base
{
    protected override void SetupSkill()
    {
        this.gameObject.GetOrAddComponent<Skill_Invinc>();
    }

    private void Start()
    {
        MoveSpeed = 2.0f;
        MaxEnergy = 10;
        CurrentEnergy = MaxEnergy;
    }
}
