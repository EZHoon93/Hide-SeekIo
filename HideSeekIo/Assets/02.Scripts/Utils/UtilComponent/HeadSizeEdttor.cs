using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadSizeEdttor : MonoBehaviour
{
    public float size = .8f;

    [ContextMenu("Change")]
    public void Change()
    {
        var headTr = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);
        headTr.transform.localScale = new Vector3(size,size,size);
    }
}
