
using System;

using BehaviorDesigner.Runtime;
namespace BehaviorDesigner.Runtime.Tasks.Unity
{
    public class SharedLivingEntity : SharedVariable<LivingEntity>
    {
        public static implicit operator SharedLivingEntity(LivingEntity value) { return new SharedLivingEntity { Value = value }; }

    }
}