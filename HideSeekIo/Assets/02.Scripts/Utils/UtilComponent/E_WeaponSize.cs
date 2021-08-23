using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_WeaponSize : MonoBehaviour
{
    public Vector3 size;
    public Transform targetTr;


    [ContextMenu("Setup")]
    public void Setup()
    {
        this.transform.GetChild(0).transform.localRotation = targetTr.GetChild(0).localRotation;
        this.transform.GetChild(0).transform.localPosition = targetTr.GetChild(0).localPosition;
        this.transform.GetChild(0).transform.localScale = targetTr.GetChild(0).localScale;

    }
}
