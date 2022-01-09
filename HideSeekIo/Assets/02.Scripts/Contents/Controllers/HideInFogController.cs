
using FoW;
using UnityEngine;


[RequireComponent(typeof (HideInFog))  ]
public class HideInFogController : MonoBehaviour
{
    HideInFog _hideInFog;

    private void Awake()
    {
        _hideInFog = GetComponent<HideInFog>();
    }

    private void OnEnable()
    {
        Managers.CameraManager.AddHideInFogController(this);
    }

    private void OnDisable()
    {
        Managers.CameraManager.RemoveHideInFogController(this);
    }

    public void UpdateHideInFog() => _hideInFog.UpdateInFog();

    public void ChangeTeam(int team) => _hideInFog.team = team;
    
}
