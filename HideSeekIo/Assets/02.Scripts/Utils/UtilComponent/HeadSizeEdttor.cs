using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadSizeEdttor : MonoBehaviour
{
    private void Reset()
    {
        var headTr = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);
        headTr.transform.localScale = new Vector3(0.8f, .8f, .8f);
    }
}
