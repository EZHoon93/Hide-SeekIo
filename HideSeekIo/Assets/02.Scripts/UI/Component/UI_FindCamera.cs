using System.Collections;

using UnityEngine;

public class UI_FindCamera : MonoBehaviour
{
    private void Awake()
    {
        this.gameObject.SetActive(false);
 
    }
    void Start()
    {
        PhotonGameManager.Instacne.AddListenr(Define.GameState.Wait, () => SetActive(false));
        PhotonGameManager.Instacne.AddListenr(Define.GameState.CountDown, () => SetActive(false));
        PhotonGameManager.Instacne.AddListenr(Define.GameState.Gameing, Check);
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
