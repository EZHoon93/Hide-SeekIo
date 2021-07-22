using System.Collections;

using UnityEngine;

public class UI_FindCamera : MonoBehaviour
{
    void Start()
    {
        PhotonGameManager.Instacne.AddListenr(Define.GameState.Wait, () => SetActive(false));
        PhotonGameManager.Instacne.AddListenr(Define.GameState.CountDown, () => SetActive(false));
        PhotonGameManager.Instacne.AddListenr(Define.GameState.Gameing, Check);

        switch (PhotonGameManager.Instacne.State)
        {
            case Define.GameState.Wait:
            case Define.GameState.CountDown:
            case Define.GameState.GameReady:
                this.gameObject.SetActive(false);
                break;
            case Define.GameState.Gameing:
            case Define.GameState.End:
                this.gameObject.SetActive(true);
                break;
        }
    }

    void SetActive(bool active)
    {
        this.gameObject.SetActive(false);
    }

    void Check()
    {
        if(Managers.Game.myPlayer == null)
        {
            this.gameObject.SetActive(true);

        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
