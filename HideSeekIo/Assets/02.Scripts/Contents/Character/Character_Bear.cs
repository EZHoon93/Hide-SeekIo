using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Bear : Character_Base
{
    protected override void SetupSkill()
    {
        mainSkill = this.gameObject.GetOrAddComponent<Skill_Invinc>();
    }

}
