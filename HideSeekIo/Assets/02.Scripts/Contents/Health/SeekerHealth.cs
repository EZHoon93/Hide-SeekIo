using System.Collections;

using UnityEngine;

public class SeekerHealth : LivingEntity
{
    protected override void Awake()
    {
        base.Awake();
        Team = Define.Team.Seek;
    }
}
