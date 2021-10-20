using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass2 : MonoBehaviour 
{
    [SerializeField] Material _orignalMaterial;
    [SerializeField] Material _transMaterial;

    [SerializeField] List<PlayerController> playerControllerList = new List<PlayerController>(16);

    Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }
    private void Start()
    {
        _renderer.material = _orignalMaterial;
    }
    public void SetActiveByGrassDetected(bool active)
    {
        if (active)
        {
            _renderer.material = _transMaterial;
        }
        else
        {
            _renderer.material = _orignalMaterial;
        }
    }


    public void Enter(PlayerController enterPlayer, Collider collider)
    {
        enterPlayer.fogOfWarController.hideInFog.isInGrass = true;
        if (playerControllerList.Contains(enterPlayer) == false)
        {
            playerControllerList.Add(enterPlayer);
        }

        //같은부쉬에 들어왔을 시 
        if (CameraManager.Instance.cameraTagerPlayer)
        {
            if (playerControllerList.Contains(CameraManager.Instance.cameraTagerPlayer))
            {
                enterPlayer.fogOfWarController.hideInFog.isGrassDetected = true;
            }
        }
        if (CameraManager.Instance.cameraTagerPlayer != enterPlayer) return;
        //SeePlayersInGrass(true);
    }



    public void Exit(PlayerController exitPlayer, Collider collider)
    {
        exitPlayer.fogOfWarController.hideInFog.isInGrass = false;
        exitPlayer.fogOfWarController.hideInFog.isGrassDetected = false;
        if (playerControllerList.Contains(exitPlayer))
        {
            playerControllerList.Remove(exitPlayer);
        }
        if (CameraManager.Instance.cameraTagerPlayer != exitPlayer) return;

        //SeePlayersInGrass(false);
    }
    void SeePlayersInGrass(bool see)
    {
        foreach (var player in playerControllerList)
            player.fogOfWarController.hideInFog.isGrassDetected = see;
    }

}
