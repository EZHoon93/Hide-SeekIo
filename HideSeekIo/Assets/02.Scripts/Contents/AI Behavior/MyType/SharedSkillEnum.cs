
using System;

using BehaviorDesigner.Runtime;

public class SharedSkillEnum : SharedVariable<Define.Skill>
{
    public static implicit operator SharedSkillEnum(Define.Skill value) { return new SharedSkillEnum { Value = value }; }

}
