using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using System.Linq;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class GameManager  
{
    public Action GameResetEvent;   //게임 리셋, 유저 참여가 다없을시 발동,
    public PlayerController myPlayer { get; set; }

    Dictionary<int, LivingEntity> _livingEntityDic = new Dictionary<int, LivingEntity>();
    public GameScene CurrentGameScene { get; set; }

  
  
    public void GameJoin()
    {
        //UIM
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "jn", true } });
    }
    public void GameExit()
    {
        if(myPlayer)
        {
            PhotonNetwork.Destroy(myPlayer.gameObject);
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "jn", false } });
        CameraManager.Instance.ResetCamera();
        InputManager.Instacne.OffAllController();
        GameResetEvent?.Invoke();   
    }
    public void GameReset()
    {
        GameResetEvent?.Invoke();
    }
    public void Clear()
    {

    }

    #region LivingEntity Register&UnRegister, Get

    public void RegisterLivingEntity(int viewID, LivingEntity livingEntity)
    {
        if (_livingEntityDic.ContainsKey(viewID)) return;
        _livingEntityDic.Add(viewID, livingEntity);
    }

    public void UnRegisterLivingEntity(int viewID, LivingEntity livingEntity)
    {
        if (!_livingEntityDic.ContainsKey(viewID)) return;
        _livingEntityDic.Remove(viewID);
    }

    public LivingEntity GetLivingEntity(int viewID)
    {
        if (_livingEntityDic.ContainsKey(viewID))
        {
            return _livingEntityDic[viewID];
        }

        return null;
    }
    public LivingEntity[] GetAllLivingEntity()
    {
        return _livingEntityDic.Values.ToArray();
    }
    public LivingEntity[] GetAllSeekerList()
    {
        return _livingEntityDic.Where(s => s.Value.Team == Define.Team.Hide).Select(s => s.Value).ToArray();
    }
    public LivingEntity[] GetAllHiderList()
    {
        return _livingEntityDic.Where(s => s.Value.Team == Define.Team.Hide).Select(s => s.Value).ToArray();
    }

    //살아있는 Hider 수 
    public int GetHiderCount()
    {
        return _livingEntityDic.Count(s => s.Value.Team == Define.Team.Hide && s.Value.Dead == false);
    }

    public int GetSeekerCount()
    {
        return _livingEntityDic.Count(s => s.Value.Team == Define.Team.Seek && s.Value.Dead == false);
    }
    #endregion


}
