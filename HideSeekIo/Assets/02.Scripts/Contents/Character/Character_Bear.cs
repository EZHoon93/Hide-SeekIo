using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Bear : Character_Base
{
    protected override void SetupSkill()
    {
        //this.gameObject.GetOrAddComponent<Skill_Invinc>();
        this.gameObject.GetOrAddComponent<Skill_Dash>();

    }

    private void Start()
    {
        MoveSpeed = 2.5f;
        MaxEnergy = 10;
        CurrentEnergy = MaxEnergy;
    }
}
