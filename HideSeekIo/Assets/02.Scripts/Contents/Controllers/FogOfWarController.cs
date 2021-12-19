using UnityEngine;
using FoW;
using Photon.Pun;
public class FogOfWarController : MonoBehaviour
{
    public FogOfWarTeam fogOfWarTeam { get; private set; }
    public FogOfWarUnit fogOfWarUnit { get; private set; }
    public HideInFog hideInFog { get; private set; }
    LivingEntity _livingEntity;
    float _ratio;
    float _initCircleRadius;
    public float ratio
    {
        get => _ratio;
        set
        {
            _ratio = value;
            fogOfWarUnit.circleRadius = _initCircleRadius * _ratio;
        }
    }

    private void Awake()
    {
        fogOfWarTeam = GetComponent<FogOfWarTeam>();
        fogOfWarUnit = GetComponent<FogOfWarUnit>();
        hideInFog = GetComponent<HideInFog>();
    }

    private void OnEnable()
    {
        if (Managers.cameraManager.cameraTagerPlayer)
        {
            hideInFog.team = Managers.cameraManager.cameraTagerPlayer.ViewID();
        }
        Managers.cameraManager.fogChangeEvent += ChangeCameraTarget;
    }

    private void OnDisable()
    {
        if (Managers.cameraManager == null) return;
        Managers.cameraManager.fogChangeEvent -= ChangeCameraTarget;
    }
    public void OnPhotonInstantiate(LivingEntity livingEntity)
    {
        _livingEntity = livingEntity;
        fogOfWarTeam.team = _livingEntity.photonView.ViewID;
        fogOfWarUnit.team = _livingEntity.photonView.ViewID;
        _initCircleRadius = fogOfWarUnit.circleRadius;
        hideInFog.isInGrass = false;
        hideInFog.isGrassDetected= false;
        hideInFog.istransSkill= false;
        CheckIsCamaeraTarget(false);
        ratio = 1;
    }

    void ChangeCameraTarget(int cameraViewID)
    {
        hideInFog.team = cameraViewID;
        var playerController = _livingEntity.GetComponent<PlayerController>();
        bool isCurrCameraTarget = cameraViewID == fogOfWarTeam.team ? true : false; //현재 카메라가 보고있는 오브젝트라면

        CheckIsCamaeraTarget(isCurrCameraTarget);
        //카메
        if (playerController)
        {
            playerController.playerGrassDetect.gameObject.SetActive(isCurrCameraTarget);
            playerController.fogOfWarController.hideInFog.isGrassDetected = isCurrCameraTarget;
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

    public void ChangeTransParentBySkill(bool isTransParent)
    {
        hideInFog.ChangeTransParentBySkill(isTransParent, _livingEntity.Team);
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
