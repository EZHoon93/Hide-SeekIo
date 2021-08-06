using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Dog : Character_Base
{
    protected override void SetupSkill()
    {
        _mainSkill = this.gameObject.GetOrAddComponent<Skill_Track>();
    }

    protected override void SetupUI()
    {

    }
}
