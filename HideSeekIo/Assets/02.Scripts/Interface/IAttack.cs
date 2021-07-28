using System.Collections;

using UnityEngine;

public interface IAttack 
{
      bool Zoom(Vector2 inputVector);
      void Attack(Vector2 inputVector);
      bool  AttackCheck(Vector2 inputVector);

}
