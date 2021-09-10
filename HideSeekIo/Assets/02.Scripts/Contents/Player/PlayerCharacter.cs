using System.Collections;

using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{

    public Animator animator => _characterAvater.animator;
    //public CharacterAvater characterAvater { get; set; }
    public Character_Base character_Base { get; private set; }
    public CharacterAvater characterAvater => _characterAvater;

    CharacterAvater _characterAvater;


    public void OnPhotonInstantiate(PlayerController _playerController)
    {
        character_Base.OnPhotonInstantiate(_playerController);
    }

    public void ChangeOnwerShip(PlayerController _playerController)
    {
        character_Base.ChangeOnwerShip(_playerController);
    }
    public Character_Base CreateCharacter(Define.CharacterType characterType, string avaterID)
    {
        if (character_Base)
        {
            Managers.Resource.Destroy(character_Base.gameObject);
        }
        character_Base = Managers.Resource.Instantiate($"Contents/{characterType.ToString()}").GetComponent<Character_Base>();
        character_Base.transform.ResetTransform(this.transform);

        CreateAvater(characterType, avaterID);

        return character_Base;
    }

    public void CreateAvater(Define.CharacterType characterType, string avaterID)
    {
        if (_characterAvater)
        {
            Managers.Resource.Destroy(_characterAvater.gameObject);
        }
        string prefabID = $"Character/{characterType.ToString()}/{avaterID}";
        _characterAvater = Managers.Resource.Instantiate(prefabID).GetComponent<CharacterAvater>();
        _characterAvater.transform.ResetTransform(character_Base.transform);
        _characterAvater.transform.localScale = Vector3.one * 2;
    }

    public RenderController GetRenderController()
    {
        return _characterAvater.renderController;
    }
}
