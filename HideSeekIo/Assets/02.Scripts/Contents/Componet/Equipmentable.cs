using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipmentable : MonoBehaviour
{
    //[SerializeField] Transform _model;
    //[SerializeField] Define.SkinType _skinType;
    public Transform targetModel { get; private set; }
    public Define.SkinType skinType { get; private set; }

    public void Setup(Transform _targetModel , Define.SkinType _skinType)
    {
        targetModel = _targetModel;
        skinType = _skinType;
    }
   
    public void OnPhotonInstantiate(PlayerController playerController)
    {
        //Managers.InputItemManager.SetupEquipmentable(playerController, this);
        var avater = playerController.playerCharacter.characterAvater;
        var adaptTransform = avater.GetSkinParentTransform(skinType);
        targetModel.transform.ResetTransform(adaptTransform);
    }

    public void OnDestroyEvent()
    {
        if (targetModel)
        {
            targetModel.transform.SetParent(this.transform);
        }
    }

}
