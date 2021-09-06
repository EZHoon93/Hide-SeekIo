using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderController : MonoBehaviour
{
  [SerializeField]  Renderer[] _renderers;

    public Renderer[] renderers => _renderers;

    private void Reset()
    {
        _renderers = GetComponentsInChildren<Renderer>();
    }
    [ContextMenu("Setup")]
    public void Setup()
    {
        _renderers = GetComponentsInChildren<Renderer>();
    }

}
