using System.Collections;

using UnityEngine;

public class Grass : MonoBehaviour , ICanEnterTriggerPlayer, IExitTrigger, IGrassDetected
{
    [SerializeField] Material _orignalMaterial;
    [SerializeField] Material _transMaterial;
    [SerializeField] Renderer _renderer;
    private void Start()
    {
        _renderer.material = _orignalMaterial;
    }
    public void ChangeTransParent(bool active)
    {
        if (active)
            _renderer.material = _transMaterial;
        else
            _renderer.material = _orignalMaterial;
    }

    public void Enter(PlayerController enterPlayer, Collider collider)
    {
         enterPlayer.isGrass = true;
    }


    public void Exit(GameObject exitGameObject)
    {
        var playerController = exitGameObject.GetComponent<PlayerController>();
        if (playerController)
        {
            playerController.isGrass = false;
        }
    }

    
}
