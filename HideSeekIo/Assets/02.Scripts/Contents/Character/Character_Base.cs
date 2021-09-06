
using System.Collections.Generic;

using Photon.Pun;
using UnityEngine;

public abstract class Character_Base : MonoBehaviour
{
    public Define.CharacterType characterType{ get; set; }
    public PlayerController playerController { get; set; }
    public List<Renderer> renderers { get; private set; } = new List<Renderer>();

    public float MaxEnergy { get; protected set; } = 10;
    public float CurrentEnergy { get; set; } = 10;
    public float MoveSpeed { get; protected set; } = 2;

    protected abstract void SetupSkill();
  
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void OnPhotonInstantiate()
    {
        SetupSkill();
    }

    public void ChangeOnwerShip()
    {
    }
    
}
