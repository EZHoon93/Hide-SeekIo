using System.Collections;
using System.Collections.Generic;

using Photon.Pun;

using UnityEngine;
using UnityEngine.SceneManagement;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SceneManagerEx
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Define.Scene type)
    {
        Managers.Clear();
        if( (int)type > 20)
        {
            // PhotonNetwork.LoadLevel(GetSceneName(type));
            PhotonNetwork.LoadLevel("Main1");
            //PhotonNetwork.LoadLevel("Main2");

        }
        else
        {
            SceneManager.LoadScene(GetSceneName(type));
        }
    }

    public void LoadSceneByIndex(int index)
    {
        Define.Scene loadSceneType = (Define.Scene)Util.GetEnumByIndex<Define.Scene>(index);
        LoadScene(loadSceneType);
    }

    public void MasterSelectNextMainScene(Define.Scene currentSceneType)
    {
        if (PhotonNetwork.IsMasterClient == false) return;
        Define.Scene nextScene = Define.Scene.Unknown;
        do
        {
            nextScene = (Define.Scene)Util.RandomEnum<Define.Scene>(currentSceneType);
        } while ((int)nextScene < 20 || (int)nextScene > 50);


        PhotonNetwork.CurrentRoom.SetCustomProperties(
            new Hashtable() { { "map", (int)nextScene } }
        );



    }
    public void LoadGunScene(string loadScene = null)
    {

    }
    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
