using System.Collections;

using UnityEngine;
using Photon.Pun;
public class Weapon_Stone : Weapon_Throw
{
    protected override  void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        Setup("Throw", .2f, .3f, 5.0f, 1);
    }
}
