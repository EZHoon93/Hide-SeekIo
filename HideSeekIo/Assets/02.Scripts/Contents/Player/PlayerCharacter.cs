using Photon.Pun;
using UnityEngine;

public class PlayerCharacter : MonoBehaviourPun
{
    CharacterAvater _characterAvater;
    LivingEntity _livingEntity;


    public CharacterAvater characterAvater => _characterAvater;
    public Animator animator => _characterAvater.animator;


    private void Awake()
    {
        _livingEntity = GetComponent<LivingEntity>();
    }


    public void OnPhotonInstantiate(PlayerController playerController)
    {
        _characterAvater.OnPhotonInstantiate(playerController);
        _characterAvater.gameObject.SetActive(true);
    }
    public virtual void OnPreNetDestroy(PhotonView rootView)
    {

    }
   
    public void ChangeOnwerShip(PlayerController _playerController)
    {
        
    }
    public void CreateAvaterByIndex(int index)
    {
        _characterAvater = Managers.Spawn.GetSkinByIndex(Define.ProductType.Character, index).GetComponent<CharacterAvater>();
        _characterAvater.transform.ResetTransform(_livingEntity.fogController.transform);
    }

    public RenderController GetRenderController()
    {
        return _characterAvater.renderController;
    }

   
}
