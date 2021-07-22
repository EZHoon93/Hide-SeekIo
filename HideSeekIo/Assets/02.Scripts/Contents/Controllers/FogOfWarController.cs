using UnityEngine;
using FoW;
using Photon.Pun;
public class FogOfWarController : MonoBehaviour
{
    public FogOfWarTeam _fogOfWarTeam { get; private set; }
    public FogOfWarUnit _fogOfWarUnit { get; private set; }
    public HideInFog _hideInFog { get; private set; }

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
        if (CameraManager.Instance == null) return;
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
        //카메라로 타겟팅되었을떄 투명부분 풀고 보여줌
        if (isTargetInCamera)
        {
            _hideInFog.SetActiveRender(true);
        }
    }

    public void ChangeTransParent(bool isTransParent)
    {
        _hideInFog.ChangeTransParent(isTransParent);
    }

    public void AddHideRender(Renderer renderer)
    {
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
