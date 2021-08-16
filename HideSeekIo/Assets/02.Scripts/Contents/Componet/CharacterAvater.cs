using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CharacterAvater : MonoBehaviour
{
    [SerializeField] Transform _rightHandTransform;
    [SerializeField] Transform _accessoriesTransform;
    [SerializeField] Transform _headTransform;
    [SerializeField] Transform _weaponTransform;

    public Transform RightHandAmount => _rightHandTransform;//무기 위치할 곳 

    public Transform accessoriesTransform => _accessoriesTransform;
    public Transform headTransform => _headTransform;
    public Transform weaponTransform => _weaponTransform;

    Dictionary<Define.SkinType, GameObject> skinDic { get; set; } = new Dictionary<Define.SkinType, GameObject>();

    [ContextMenu("Setup")]
    public void Setup()
    {
        //var prefab=  Resources.Load<GameObject>("Prefabs/AvaterRightHand").transform;
        //var rightHandTransformPanel = Instantiate(prefab).transform;
        //rightHandTransformPanel.gameObject.SetActive(true);
        //var righthand = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
        //rightHandTransformPanel.ResetTransform(righthand);

        //_rightHandTransform = rightHandTransformPanel.GetComponent<RightHand>().RightHandTransform;

         _accessoriesTransform = transform.MyFindChild("Accessories_locator");
        _headTransform = transform.MyFindChild("Head_Accessories_locator");
        _weaponTransform = transform.MyFindChild("WeaponR_locator");


    }

    public void AddSkinObject(Define.SkinType skinType, GameObject newSkinObject)
    {
        GameObject skinObject;
        bool isExist = skinDic.TryGetValue(skinType, out skinObject);
        if(isExist == false)
        {
            skinDic.Add(skinType, newSkinObject);
        }
        skinObject?.gameObject.SetActive(false);
        skinObject = newSkinObject;
    }

    public void AllSkinSetup()
    {
        foreach(var skin in skinDic)
        {
            Transform parentTransform = GetSkilParentTransform(skin.Key);
            skin.Value.transform.ResetTransform(parentTransform);
        }
    }

    Transform GetSkilParentTransform(Define.SkinType skinType)
    {
        switch (skinType)
        {
            case Define.SkinType.Weapon:
                return _weaponTransform;
            case Define.SkinType.Head:
                return _headTransform;
            case Define.SkinType.Bag:
                return _accessoriesTransform;
        }

        return null;
    }
}
