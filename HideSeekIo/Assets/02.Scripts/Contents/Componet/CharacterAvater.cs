using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class CharacterAvater : MonoBehaviour
{
    [SerializeField] Transform _accessoriesTransform;
    [SerializeField] Transform _headTransform;
    [SerializeField] Transform _weaponTransform;
    [SerializeField] RenderController _renderController;
    public Animator animator { get; set; }
    public Transform accessoriesTransform => _accessoriesTransform;
    public Transform headTransform => _headTransform;
    public Transform weaponTransform => _weaponTransform;
    public RenderController renderController => _renderController;
    public Dictionary<Define.SkinType, GameObject> skinDic { get; set; } = new Dictionary<Define.SkinType, GameObject>();


    [ContextMenu("Setup")]
    public void Setup()
    {
         _accessoriesTransform = transform.MyFindChild("Accessories_locator");
        _headTransform = transform.MyFindChild("Head_Accessories_locator");
        _weaponTransform = transform.MyFindChild("WeaponR_locator");
        _renderController = this.gameObject.GetOrAddComponent<RenderController>();
        _renderController.Setup();

    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = GameSetting.Instance.playerAnimator;
        animator.applyRootMotion = false;
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
        skinObject.transform.ResetTransform(GetSkilParentTransform(skinType));
        skinObject.gameObject.SetActive(true);
    }

    public void AllSkinSetup()
    {
        foreach(var skin in skinDic)
        {
            Transform parentTransform = GetSkilParentTransform(skin.Key);
            skin.Value.transform.ResetTransform(parentTransform);
        }
    }

    public Transform GetSkilParentTransform(Define.SkinType skinType)
    {
        switch (skinType)
        {
            case Define.SkinType.Weapon:
                return _weaponTransform;
            case Define.SkinType.Hat:
                return _headTransform;
            case Define.SkinType.Bag:
                return _accessoriesTransform;
            case Define.SkinType.Skin:
                return this.transform;
        }

        return null;
    }

    public void CloneSkin(Dictionary<Define.SkinType , GameObject> cloneData)
    {
        foreach (var c in skinDic)
            c.Value.gameObject.SetActive(false);

        skinDic.Clear();
        skinDic = cloneData.ToDictionary(x => x.Key, y => y.Value);

        AllSkinSetup();
    }
}
