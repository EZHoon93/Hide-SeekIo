using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipmentable : MonoBehaviour
{
    public Define.SkinType equipSkiType { get; set; }

    public void Setup(Define.SkinType skinType )
    {
        equipSkiType = skinType;
    }
}
