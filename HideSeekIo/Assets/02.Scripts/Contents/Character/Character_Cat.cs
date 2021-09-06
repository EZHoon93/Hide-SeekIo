using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Cat : Character_Base
{
    protected override void SetupSkill()
    {
        this.gameObject.GetOrAddComponent<Skill_Stealth>();

    }


}
