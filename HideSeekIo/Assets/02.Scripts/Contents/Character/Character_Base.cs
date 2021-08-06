
using Photon.Pun;
using UnityEngine;

public abstract class Character_Base : MonoBehaviour
{
    [SerializeField] GameObject avaterObject;
    [SerializeField] Transform _rightHandTransform;

    public Transform RightHandAmount => _rightHandTransform;//무기 위치할 곳 


    protected Skill_Base _mainSkill;


    private void Awake()
    {
        //GetComponent<IOnPhotonInstantiate>().OnPhotonInstantiateEvent += IOnPhotonInstantiate;
        SetupSkill();
    }

    protected abstract void SetupSkill();


    void IOnPhotonInstantiate(PhotonView photonView)
    {
        SetupUI();
    }

    protected virtual void SetupUI()
    {

    }
 

    [ContextMenu("Setup")]
    public void Setup()
    {
        var prefab = Resources.Load<GameObject>("Prefabs/AvaterRightHand").transform;
        var rightHandTransformPanel = Instantiate(prefab).transform;
        rightHandTransformPanel.gameObject.SetActive(true);
        var righthand = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
        rightHandTransformPanel.ResetTransform(righthand);

        _rightHandTransform = rightHandTransformPanel.GetComponent<RightHand>().RightHandTransform;


    }

    //protected void 
}
