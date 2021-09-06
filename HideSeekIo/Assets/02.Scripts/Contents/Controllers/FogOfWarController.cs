using UnityEngine;
using FoW;
using Photon.Pun;
public class FogOfWarController : MonoBehaviour
{
    public FogOfWarTeam fogOfWarTeam { get; private set; }
    public FogOfWarUnit fogOfWarUnit { get; private set; }
    public HideInFog hideInFog { get; private set; }

    private void Awake()
    {
        fogOfWarTeam = GetComponent<FogOfWarTeam>();
        fogOfWarUnit = GetComponent<FogOfWarUnit>();
        hideInFog = GetComponent<HideInFog>();

        this.transform.parent.GetComponent<IOnPhotonInstantiate>().OnPhotonInstantiateEvent += OnPhotonInstantiate;
    }
    private void OnEnable()
    {
        if (CameraManager.Instance.Target)
        {
            hideInFog.team = CameraManager.Instance.Target.ViewID();
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
        fogOfWarTeam.team = photonView.ViewID;
        fogOfWarUnit.team = photonView.ViewID;
        CheckIsCamaeraTarget(false);
    }

    void ChangeCameraTarget(int cameraViewID)
    {
        hideInFog.team = cameraViewID;

        if(cameraViewID == fogOfWarTeam.team)
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
        fogOfWarTeam.enabled = isTargetInCamera;
        fogOfWarTeam.enabled = isTargetInCamera;
        hideInFog.enabled = !isTargetInCamera;
        //카메라로 타겟팅되었을떄 투명부분 풀고 보여줌
        if (isTargetInCamera)
        {
            hideInFog.SetActiveRender(true);
        }
    }

    public void ChangeTransParent(bool isTransParent)
    {
        hideInFog.ChangeTransParent(isTransParent);
    }

    public void AddHideRender(RenderController renderController)
    {
        if (renderController == null) return;
        hideInFog.AddRenderer(renderController);
    }

    public void RemoveRenderer(RenderController renderController)
    {
        if (renderController == null) return;
        hideInFog.RemoveRenderer(renderController);
    }

    public void ChangeSight(float value)
    {
        fogOfWarUnit.circleRadius = value;
    }
}
