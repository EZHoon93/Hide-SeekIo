using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipmentable : MonoBehaviour
{
    [SerializeField] Transform _model;
    public Define.SkinType equipSkiType { get; set; }


    public void Setup(Define.SkinType skinType )
    {
        equipSkiType = skinType;
    }
}
