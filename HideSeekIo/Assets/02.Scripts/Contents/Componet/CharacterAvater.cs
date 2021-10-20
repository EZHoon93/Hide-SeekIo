using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class CharacterAvater : MonoBehaviour
{
    [SerializeField] Transform _accessoriesTransform;
    [SerializeField] Transform _headTransform;
    [SerializeField] Transform _leftHand;
    [SerializeField] Transform _rightHand;
    [SerializeField] RenderController _renderController;

    [SerializeField] Transform _le;
    [SerializeField] Transform _ri;
    [SerializeField] Transform _weaponPivot;

    public Animator animator { get; set; }
    public Transform accessoriesTransform => _accessoriesTransform;
    public Transform headTransform => _headTransform;
    public Transform rightHand => _rightHand;
    public Transform leftHand => _leftHand;
    public RenderController renderController => _renderController;
    public Dictionary<Define.SkinType, GameObject> skinDic { get; set; } = new Dictionary<Define.SkinType, GameObject>();

    PlayerController _playerController;

    [ContextMenu("Setup")]
    public void Setup()
    {
        //_accessoriesTransform = transform.MyFindChild("Accessories_locator");
        //_headTransform = transform.MyFindChild("Head_Accessories_locator");
        _rightHand = transform.MyFindChild("WeaponR_locator");
        //_rightHand = go.transform;
        _rightHand.localPosition = new Vector3(0.219f, 0.078f, 0);
        _rightHand.localRotation = Quaternion.Euler(new Vector3(-8, 210, -203));
        _leftHand.localScale = Vector3.one;
        //_renderController = this.gameObject.GetOrAddComponent<RenderController>();
        //_renderController.Setup();

        //if(_accessoriesTransform == null)
        //{
        //    var parTr = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Chest);
        //    var go = new GameObject("Accessories");
        //    go.transform.SetParent(parTr);
        //    _accessoriesTransform = go.transform;
        //}
        //if (_headTransform == null)
        //{
        //    var parTr = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);
        //    var go = new GameObject("Head_Accessories_locator");
        //    go.transform.SetParent(parTr);
        //    _headTransform = go.transform;
        //}
        //if (_rightHand == null)
        //{
        //    var parTr = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
        //    var go = new GameObject("WeaponR_locator");
        //    go.transform.SetParent(parTr);
        //    _rightHand = go.transform;
        //    _rightHand.localPosition = new Vector3(0.219f, 0.078f, 0);
        //    _rightHand.localRotation = Quaternion.Euler(new Vector3(-8, 210, -203));
        //    _leftHand.localScale = Vector3.one;
        //}
        //if (_leftHand == null)
        //{
        //    var parTr = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.LeftHand);
        //    var go = new GameObject("WeaponL_locator");
        //    go.transform.SetParent(parTr);
        //    _leftHand = go.transform;
        //    _leftHand.localPosition = new Vector3(0.219f, 0.078f, 0);
        //    _leftHand.localRotation = Quaternion.Euler(new Vector3(-8, 210, -203));
        //    _leftHand.localScale = Vector3.one;
        //}
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = GameSetting.Instance.playerAnimator;
        animator.applyRootMotion = false;
    }


    public void OnPhotonInstantiate(PlayerController playerController)
    {
        _playerController = playerController;
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
        skinObject.transform.ResetTransform(GetSkinParentTransform(skinType));
        skinObject.gameObject.SetActive(true);
    }

    public void AllSkinSetup()
    {
        foreach(var skin in skinDic)
        {
            Transform parentTransform = GetSkinParentTransform(skin.Key);
            skin.Value.transform.ResetTransform(parentTransform);
        }
    }

    public Transform GetSkinParentTransform(Define.SkinType skinType)
    {
        switch (skinType)
        {
            case Define.SkinType.LeftHand:
                return _leftHand;
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

    public void CloneSkin(Dictionary<Define.SkinType , GameObject> cloneData)
    {
        foreach (var c in skinDic)
            c.Value.gameObject.SetActive(false);

        skinDic.Clear();
        skinDic = cloneData.ToDictionary(x => x.Key, y => y.Value);

        AllSkinSetup();
    }

    // 애니메이터의 IK 갱신
    private void OnAnimatorIK(int layerIndex)
    {
        _playerController.playerShooter.OnAnimatorIK(layerIndex);
    }
}
