using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class CharacterAvater : MonoBehaviour
{
    [SerializeField] Transform _accessoriesTransform;
    [SerializeField] Transform _headTransform;
    [SerializeField] Transform _rightHand;
    [SerializeField] RenderController _renderController;

    public Animator animator;
    public Transform accessoriesTransform => _accessoriesTransform;
    public Transform headTransform => _headTransform;
    public Transform rightHand => _rightHand;
    public RenderController renderController => _renderController;



    PlayerController _playerController;
    public GameObject accessoriesObject { get; private set; }
    public GameObject weaponObject { get; private set; }


    [ContextMenu("Setup")]
    public void Setup()
    {
        //animator = GetComponent<Animator>();
        //animator.runtimeAnimatorController = GameSetting.Instance.playerAnimator;
        //print(animator + "!!@!@");
        //animator.applyRootMotion = false;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = Managers.GameSetting.playerAnimator;
        animator.applyRootMotion = false;
    }
    public void OnPhotonInstantiate(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public Transform GetSkinParentTransform(Define.SkinType skinType)
    {
        switch (skinType)
        {
            case Define.SkinType.RightHand:
                return _rightHand;
            case Define.SkinType.Hat:
                return _headTransform;
            case Define.SkinType.Bag:
                return _accessoriesTransform;
            case Define.SkinType.Skin:
                return this.transform;
        }

        return null;
    }

    public void SetupEquipment(Equipmentable newEquipment)
    {
        var tr = GetSkinParentTransform( newEquipment.skinType);

        newEquipment.model.transform.ResetTransform(tr);
    }

    public void SetupAccessories(GameObject newAccssories)
    {
        if (newAccssories == null) return;
        if (accessoriesObject)
        {
            Managers.Resource.Destroy(accessoriesObject.gameObject);
        }
        accessoriesObject = newAccssories;
        accessoriesObject.transform.ResetTransform(accessoriesTransform.transform);
    }

    public void SetupWeapon(GameObject newWeapon)
    {
        if(newWeapon == null)return;
        if (weaponObject)
        {
            Managers.Resource.Destroy(weaponObject.gameObject);
        }
        weaponObject = newWeapon;
        weaponObject.transform.ResetTransform(rightHand.transform);
    }

    // 애니메이터의 IK 갱신
    private void OnAnimatorIK(int layerIndex)
    {
        //_playerController.playerShooter.OnAnimatorIK(layerIndex);
    }
}
