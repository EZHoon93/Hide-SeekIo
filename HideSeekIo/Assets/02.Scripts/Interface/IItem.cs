
using System;

using UnityEngine;

public interface IItem
{
    Define.UseType useType { get; set; }

    Sprite GetSprite();
    void Zoom(Vector2 inputVector);

    void Attack(Vector2 inputVector);

}
