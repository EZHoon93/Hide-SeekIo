
using Photon.Pun;
using UnityEngine;

public abstract class Character_Base : MonoBehaviour
{
    [SerializeField] GameObject avaterObject;
    [SerializeField] Transform _rightHandTransform;

    public Animator animator { get; set; }
    public Transform RightHandAmount => _rightHandTransform;//무기 위치할 곳 


    public IAttack mainSkill { get; set; }
    public PlayerController playerController { get; set; }


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected abstract void SetupSkill();

    public void OnPhoninstiate(PlayerController newPlayerController)
    {
        playerController = newPlayerController;
        SetupSkill();
    }

    public void UseSkill(Vector2 inputVector2)
    {
        if(mainSkill.playerController == null)
        {
            mainSkill.playerController = playerController;
        }

        mainSkill.AttackCheck(inputVector2);
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
