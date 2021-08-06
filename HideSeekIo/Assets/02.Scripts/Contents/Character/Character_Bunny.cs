using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Bunny : Character_Base
{
    protected override void SetupSkill()
    {
        _mainSkill = this.gameObject.GetOrAddComponent<Skill_Stealth>();
    }

    protected override void SetupUI()
    {

    }
}
