using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipmentable : MonoBehaviour
{
    [SerializeField] Transform _model;
    [SerializeField] Define.SkinType _skinType;
    //public Transform targetModel { get; private set; }
    public Define.SkinType skinType => _skinType;
    public Transform model => _model;

    
   
    public void OnPhotonInstantiate(PlayerController playerController)
    {
        playerController.characterAvater.SetupEquipment(this);

        
    }

    public void OnDestroyEvent()
    {

    }

}
