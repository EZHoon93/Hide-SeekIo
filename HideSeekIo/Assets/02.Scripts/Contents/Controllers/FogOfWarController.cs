using UnityEngine;
using FoW;
using Photon.Pun;
public class FogOfWarController : MonoBehaviour
{
    FogOfWarTeam _fogOfWarTeam;
    FogOfWarUnit _fogOfWarUnit;
    HideInFog _hideInFog;

    private void Awake()
    {
        _fogOfWarTeam = GetComponent<FogOfWarTeam>();
        _fogOfWarUnit = GetComponent<FogOfWarUnit>();
        _hideInFog = GetComponent<HideInFog>();

        this.transform.parent.GetComponent<IOnPhotonInstantiate>().OnPhotonInstantiateEvent += OnPhotonInstantiate;
    }
    private void OnEnable()
    {
        if (CameraManager.Instance.Target)
        {
            _hideInFog.team = CameraManager.Instance.Target.ViewID();
        }
        CameraManager.Instance.fogChangeEvent += ChangeCameraTarget;
    }

    private void OnDisable()
    {
        CameraManager.Instance.fogChangeEvent -= ChangeCameraTarget;
    }
    public void OnPhotonInstantiate(PhotonView photonView)
    {
        _fogOfWarTeam.team = photonView.ViewID;
        _fogOfWarUnit.team = photonView.ViewID;
        CheckIsCamaeraTarget(false);
    }

    void ChangeCameraTarget(int cameraViewID)
    {
        _hideInFog.team = cameraViewID;

        if(cameraViewID == _fogOfWarTeam.team)
        {
            CheckIsCamaeraTarget(true);
        }
        else
        {
            CheckIsCamaeraTarget(false);
        }
    }

    void CheckIsCamaeraTarget(bool isTargetInCamera)
    {
        _fogOfWarTeam.enabled = isTargetInCamera;
        _fogOfWarTeam.enabled = isTargetInCamera;
        _hideInFog.enabled = !isTargetInCamera;
    }

    public void ChangeTransParent(bool isTransParent)
    {
        _hideInFog.ChangeTransParent(isTransParent);
    }

    public void AddHideRender(Renderer renderer)
    {
        print("추가!!! " + renderer.gameObject.name);
        if (renderer == null) return;
        _hideInFog.AddRenderer(renderer);
    }

    public void RemoveRenderer(Renderer renderer)
    {
        if (renderer == null) return;
        _hideInFog.RemoveRenderer(renderer);
    }

    public void ChangeSight(float value)
    {
        _fogOfWarUnit.circleRadius = value;
    }
}
