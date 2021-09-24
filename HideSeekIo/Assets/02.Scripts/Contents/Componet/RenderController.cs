using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터에 부착되는 렌
/// </summary>
public class RenderController : MonoBehaviour
{
    [SerializeField]  Renderer[] _renderers;
    LivingEntity _livingEntity;

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
    public void OnPhotonInstantiate(LivingEntity newLivingEntity)
    {
        //Managers.InputItemManager.SeupRenderController(livingEntity, this);
        _livingEntity = newLivingEntity;
        _livingEntity.AddRenderer(this);
    }

    public void OnDestroyEvent()
    {
        _livingEntity.RemoveRenderer(this);
    }



}
