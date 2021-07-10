using System.Collections;

using FoW;

using UnityEngine;

public class TNTProjectile : ThrowProjectileObject
{
    [SerializeField] ParticleSystem _effect;
    FogOfWarUnit _fogOfWarUnit;


    
    protected override void Explosion()
    {
        
    }
}
