using System.Collections;

using UnityEngine;

public class CharacterAvater : MonoBehaviour
{
    [SerializeField] Transform _rightHandTransform;

    public Transform RightHandAmount => _rightHandTransform;//무기 위치할 곳 


    [ContextMenu("Setup")]
    public void Setup()
    {
        var prefab=  Resources.Load<GameObject>("Prefabs/AvaterRightHand").transform;
        var rightHandTransformPanel = Instantiate(prefab).transform;
        rightHandTransformPanel.gameObject.SetActive(true);
        var righthand = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
        rightHandTransformPanel.ResetTransform(righthand);

        _rightHandTransform = rightHandTransformPanel.GetComponent<RightHand>().RightHandTransform;

        
    }
    

}
