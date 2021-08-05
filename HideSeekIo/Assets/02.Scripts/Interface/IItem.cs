
using System;

using UnityEngine;

public interface IItem
{
    Define.UseType useType { get; set; }

    //int GetInstanceID();
    Sprite GetSprite();
    Action removeCallBack { get; set; }
    //bool Zoom(object inputVector);

    //void Attack(Vector2 inputVector);

    //void Use(PlayerController usePlayerController);

    //Enum GetEnum();
}
