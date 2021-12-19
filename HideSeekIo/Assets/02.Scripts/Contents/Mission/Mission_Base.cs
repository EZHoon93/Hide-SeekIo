using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public abstract class Mission_Base : MonoBehaviourPun
{
    public abstract float missionTime { get; }
    protected UI_Mission _uI_Mission;
    public abstract void OnStart(MissionInfo missionInfo);
    public abstract void OnUpdate(int reaminTime);
    public abstract void OnTimeEnd();

    public virtual void OnDestroy()
    {

    }

    private void OnEnable()
    {
        //_uI_Mission = Managers.UI.SceneUI.GetComponent<UI_Main>().Mission;
    }


}
