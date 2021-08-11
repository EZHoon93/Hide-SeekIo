using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Bunny : Character_Base
{
    protected override void SetupSkill()
    {
        mainSkill = this.gameObject.GetOrAddComponent<Skill_Dash>();
    }

   
}
