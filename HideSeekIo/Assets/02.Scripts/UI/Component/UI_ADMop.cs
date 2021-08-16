using System.Collections;

using UnityEngine;

public class UI_ADMop : MonoBehaviour
{
    private void Awake()
    {
        this.gameObject.SetActive(true);
    }
    void Start()
    {
        //PhotonGameManager.Instacne.AddListenr(Define.GameState.Wait, () => SetActive(true));
        //PhotonGameManager.Instacne.AddListenr(Define.GameState.CountDown, () => SetActive(false));
        //PhotonGameManager.Instacne.AddListenr(Define.GameState.Gameing, Check);
        //PhotonGameManager.Instacne.AddListenr(Define.GameState.End, () => SetActive(false));
    }


    void SetActive(bool active)
    {
        this.gameObject.SetActive(false);
    }

    void Check()
    {
        if (Managers.Game.myPlayer == null)
        {
            this.gameObject.SetActive(true);

        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
