
using System.Collections.Generic;

using Photon.Pun;
using UnityEngine;

public abstract class Character_Base : MonoBehaviour
{
    public virtual Define.CharacterType characterType{ get; set; }
    public List<Renderer> renderers { get; private set; } = new List<Renderer>();

    public float MaxEnergy { get; protected set; } = 10;
    public float CurrentEnergy { get; set; } = 10;
    public float MoveSpeed { get; protected set; } = 2;

    public InputControllerObject inputControllerObject { get; set; }

  
    private void Awake()
    {
        inputControllerObject = GetComponent<InputControllerObject>();
    }

    public void OnPhotonInstantiate(PlayerController _playerController)
    {
        if (_playerController)
        {
            inputControllerObject.SetupPlayerController(_playerController);
        }
    }

    public void ChangeOnwerShip(PlayerController _playerController)
    {
        _playerController.playerShooter.SetupControllerObject(inputControllerObject);
    }

}
