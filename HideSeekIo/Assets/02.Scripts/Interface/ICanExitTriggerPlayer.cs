using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanExitTriggerPlayer
{
    void Exit(PlayerController exitPlayer, Collider collider);
}
