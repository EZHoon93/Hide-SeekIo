﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : BaseScene
{
    [SerializeField] GameObject _craeteIDPanel;


    enum State
    {
        UnLoad,
        AllLoad
    }

    State _state = State.UnLoad;

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Login;
        StartCoroutine(LoadAllData());
    }
    public override void Clear()
    {
        Debug.Log("LoginScene Clear!");
    }
    //모든 데이터를 받아올대까지 대기 
    IEnumerator LoadAllData()
    {
        PlayerInfo.Login();
        PhotonManager.Instance.Connect();
        //필요 데이터 갖고옴
        while (_state == State.UnLoad)
        {
            print(_state);
            if (GetIsAllOnLoad())
            {
                _state = State.AllLoad;
            }
            yield return new WaitForSeconds(1.0f);
        }
        Managers.Scene.LoadScene(Define.Scene.Lobby);
    }


    bool GetIsAllOnLoad()
    {
        var photonData = CheckPhoton();
        var userData = CheckPlayerInfo();
        bool gameData = CheckData();
        print(photonData + "/" + userData + "/" + gameData);

        return photonData && userData && gameData ;
    }



    bool CheckPlayerInfo()
    {
        if(PlayerInfo.State == Define.UserDataState.Load)
        {
            return true;
        }

        if(PlayerInfo.State == Define.UserDataState.Null)
        {
            //아이디없음.. 아이디만들기.
            _craeteIDPanel.SetActive(true);
            PlayerInfo.State = Define.UserDataState.Wait;
        }

        return false;
    }

   bool CheckPhoton()
    {
        if (PhotonManager.Instance.State == Define.ServerState.Connect)
        {
            return true;
        }

        return false;
    }

    bool CheckData()
    {
        if (Managers.Data.State == Define.GameDataState.Load)
        {
            return true;
        }
        return false;
    }
}
