using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class GameManager  
{
    GameStateController _gameStateController;

    public PlayerController myPlayer { get; set; }

    Dictionary<int, LivingEntity> _livingEntityDic = new Dictionary<int, LivingEntity>();
    public GameScene CurrentGameScene { get; set; }

    public GameStateController gameStateController
    {
        get => _gameStateController;
        set
        {
            if (_gameStateController)
            {
                Managers.Resource.PunDestroy(_gameStateController);
            }
            _gameStateController = value;
            _gameStateController.transform.ResetTransform(CurrentGameScene.transform);
            PhotonGameManager.Instacne?.PostStateEvent(_gameStateController.gameStateType);
        }
    }
 


    public void Clear()
    {
        var livArray = _livingEntityDic.Values.ToArray();
        foreach (var liv in livArray)
        {
            if (liv == null) continue;
            Managers.Resource.PunDestroy(liv);
        }
        _livingEntityDic.Clear();
    }

    #region LivingEntity Register&UnRegister, Get

    public void RegisterLivingEntity(int viewID, LivingEntity livingEntity)
    {
        if (_livingEntityDic.ContainsKey(viewID)) return;
        _livingEntityDic.Add(viewID, livingEntity);
        if (livingEntity.gameObject.IsValidAI())
        {
            //AIManager.Instance.RegisterAI(livingEntity);
        }
    }

    public void UnRegisterLivingEntity(int viewID )
    {
        if (!_livingEntityDic.ContainsKey(viewID)) return;
        _livingEntityDic.Remove(viewID);
        //AIManager.Instance.UnRegisterAI(viewID);
    }

    public PlayerController GetPlayerController(int viewID)
    {
        var livingEntity = GetLivingEntity(viewID);
        if (livingEntity)
        {
            return livingEntity.GetComponent<PlayerController>();
        }
        return null;
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
