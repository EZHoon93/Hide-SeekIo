using System;
using System.Collections;

using UnityEngine;

public interface IAttack 
{
    Define.ControllerType controllerType { get; set; }
    Action<IAttack> AttackSucessEvent { get; set; }
    Action AttackEndEvent { get; set; }
    PlayerController playerController { get; set; }

    float AttackDistance { get; set; }
    string AttackAnim { get; set; }
    void Zoom(Vector2 inputVector);
    bool AttackCheck(Vector2 inputVector);
    Vector3 AttackPoint { get; set; }
}
