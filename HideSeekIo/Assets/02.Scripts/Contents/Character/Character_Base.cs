
using System.Collections.Generic;

using Photon.Pun;
using UnityEngine;

public abstract class Character_Base : MonoBehaviour
{
    public Animator animator => characterAvater.animator;
    public CharacterAvater characterAvater{ get; set; }
    public Define.CharacterType characterType{ get; set; }
    public PlayerController playerController { get; set; }
    public List<Renderer> renderers { get; private set; } = new List<Renderer>();

    public float MaxEnergy { get; protected set; } 
    public float CurrentEnergy { get;  set; }
    public float MoveSpeed { get; protected set; }

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
    public CharacterAvater CreateAvater(string avaterID)
    {
        characterAvater = Managers.Spawn.CharacterAvaterSpawn(characterType, avaterID);

        return characterAvater;
    }
}
