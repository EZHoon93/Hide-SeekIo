using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Grass : MonoBehaviour , ICanEnterTriggerPlayer, ICanExitTriggerPlayer
{
    [SerializeField] Material _orignalMaterial;
    [SerializeField] Material _transMaterial;
    [SerializeField] Renderer[] _renderer;
    [SerializeField] List<PlayerController> playerControllerList = new List<PlayerController>(16);
    private void Start()
    {
        Managers.cameraManager.fogChangeEvent += CameraChange;
        _renderer = GetComponentsInChildren<Renderer>();
        foreach (var r in _renderer)
            r.material = _orignalMaterial;
    }
    public void ChangeTransParent(bool active)
    {
       
    }

    //카메라 변경p
    void CameraChange(int viewID)
    {
        if (playerControllerList.Count <= 0) return;
        foreach(var player in playerControllerList)
        {
            if(player.ViewID() == viewID)
            {
                //player.isGrass = true;
                //player.fogOfWarController.hideInFog.isGrassDetected = see;
                SeePlayersInGrass(true);
                ChangeGrassMaterial(true);
                return;
            }
        }
        SeePlayersInGrass(false);
        ChangeGrassMaterial(false);
    }
    public void SetActiveByGrassDetected(bool active)
    {
        if (active)
        {
            foreach (var r in _renderer)
                r.material = _transMaterial;
            
        }

        else
        {
            foreach (var r in _renderer)
                r.material = _orignalMaterial;
        }

        SeePlayersInGrass(active);
    }

    void ChangeGrassMaterial(bool trans)
    {
        if (trans)
        {
            foreach (var r in _renderer)
                r.material = _transMaterial;

        }

        else
        {
            foreach (var r in _renderer)
                r.material = _orignalMaterial;
        }
    }

    void SeePlayersInGrass(bool see)
    {
        foreach (var player in playerControllerList)
            player.fogOfWarController.hideInFog.isGrassDetected = see;
    }
    public void Enter(PlayerController enterPlayer, Collider collider)
    {
        //enterPlayer.fogOfWarController.hideInFog.isGrass = true;
        if(playerControllerList.Contains(enterPlayer) == false)
        {
            playerControllerList.Add(enterPlayer);
        }

        //같은부쉬에 들어왔을 시 
        if (Managers.cameraManager.cameraTagerPlayer)
        {
            if(playerControllerList.Contains(Managers.cameraManager.cameraTagerPlayer))
            {
                enterPlayer.fogOfWarController.hideInFog.isGrassDetected = true;
            }
        }
        if (Managers.cameraManager.cameraTagerPlayer != enterPlayer) return;

        SeePlayersInGrass(true);
        ChangeGrassMaterial(true);
    }



    public void Exit(PlayerController exitPlayer, Collider collider)
    {
        //exitPlayer.fogOfWarController.hideInFog.isGrass = false;
        exitPlayer.fogOfWarController.hideInFog.isGrassDetected = false;
        if (playerControllerList.Contains(exitPlayer))
        {
            playerControllerList.Remove(exitPlayer);
        }
        if (Managers.cameraManager.cameraTagerPlayer != exitPlayer) return;

        SeePlayersInGrass(false);
        ChangeGrassMaterial(false);

    }


}
