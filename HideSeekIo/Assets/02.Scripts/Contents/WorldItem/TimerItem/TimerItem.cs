using UnityEngine;
using Photon.Pun;
using FoW;
public abstract class TimerItem : MonoBehaviour
{
    protected TimerItemController _timerItemController;
    protected FogOfWarUnit _fogOfWarUnit;
    protected HideInFog _hideInFog;
    private void Awake()
    {
        _timerItemController = GetComponent<TimerItemController>();
        _fogOfWarUnit = GetComponent<FogOfWarUnit>();
        _hideInFog = GetComponent<HideInFog>();
    }
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info )
    {
        if (_timerItemController.usePlayer)
        {
            _fogOfWarUnit.team = _timerItemController.usePlayer.ViewID();
            if (_timerItemController.usePlayer.IsMyCharacter())
            {
                _hideInFog.SetActiveRender(true);
                _hideInFog.enabled = false;
            }
            else
            {
                _hideInFog.enabled = true;
            }
        }

    }
    public virtual void EndTime()
    {

    }
}
