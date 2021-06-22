using System.Collections;

using UnityEngine;

public class SeekerHealth : LivingEntity
{
    private void Awake()
    {
        Team = Define.Team.Seek;
    }
}
