using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using ExitGames.Client.Photon.StructWrapping;

public class PlayerStat : Stat
{
    CharacterAvater _characterAvater;

    public CharacterAvater characterAvater => _characterAvater;

    public void SetupCharacterAvater(GameObject avaterObject)
    {
        var avater = avaterObject.GetComponent<CharacterAvater>();
        if (avater == null)
        {
            return;
        }

        _characterAvater = avater;
        _animator = _characterAvater.animator;
    }
   


}


