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
        _livingEntity = newLivingEntity;
        _livingEntity.AddRenderer(this);
    }

    public void OnDestroyEvent()
    {
        print("제거!! ㄷ;스.." + _renderers.Count+ this.gameObject.name);
        _livingEntity.RemoveRenderer(this);
    }



}
