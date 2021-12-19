using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

/// <summary>
/// 캐릭터에 부착되는 렌
/// </summary>
public class RenderController : MonoBehaviour
{
    [SerializeField]  List< Renderer >_renderers = new List<Renderer>(8);
    LivingEntity _livingEntity;

    public List<Renderer> renderers => _renderers;

    private void Reset()
    {
        _renderers = GetComponentsInChildren<Renderer>().ToList();
    }
    [ContextMenu("Setup")]
    public void Setup()
    {
        _renderers = GetComponentsInChildren<Renderer>().ToList();
    }
    public void OnPhotonInstantiate(LivingEntity newLivingEntity)
    {
        //Managers.InputItemManager.SeupRenderController(livingEntity, this);
        foreach(var r in _renderers)
        {
            r.enabled = true;
        }
        _livingEntity = newLivingEntity;
        _livingEntity.AddRenderer(this);
    }

    public void OnDestroyEvent()
    {
        _livingEntity.RemoveRenderer(this);
    }



}
