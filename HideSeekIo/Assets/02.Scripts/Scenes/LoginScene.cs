using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : BaseScene
{
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

        StartCoroutine(LoadData());
    }
    public override void Clear()
    {
        Debug.Log("LoginScene Clear!");
    }
    IEnumerator LoadData()
    {
        PlayerInfo.Login();
        PhotonManager.instacne.Connect();
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
        bool photonData = false;
        bool userData = false;
        bool gameData = false;
        if (PhotonManager.instacne.State == Define.ServerState.Connect)
            photonData = true;
        if (PlayerInfo.State == Define.UserDataState.Load)
            userData = true;
        if(Managers.Data.State == Define.GameDataState.Load)
            gameData = true;


        print(photonData + "/" + userData + "/" + gameData);

        return photonData && userData && gameData ;
    }


   
}
