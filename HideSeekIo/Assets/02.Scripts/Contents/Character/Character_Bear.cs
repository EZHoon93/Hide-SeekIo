﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Bear : Character_Base
{
    protected override void SetupSkill()
    {
        _mainSkill = this.gameObject.GetOrAddComponent<Skill_Invinc>();

    }

    protected override void SetupUI()
    {

    }
}
