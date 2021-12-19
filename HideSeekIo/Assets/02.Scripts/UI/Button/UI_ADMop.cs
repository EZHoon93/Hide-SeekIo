using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class UI_ADMop : MonoBehaviour
{
    [SerializeField] Button _cancelButton;
    [SerializeField] Button _showButton;

    private void Awake()
    {
        SetActive(false);
        _cancelButton.onClick.AddListener(OnClick_Cancel);
        _showButton.onClick.AddListener(OnClick_Show);


    }
    void Start()
    {
        Managers.Game.AddListenrOnGameState(Define.GameState.Wait, () => SetActive(true));
        Managers.Game.AddListenrOnGameState(Define.GameState.CountDown, () => SetActive(false));
        Managers.Game.AddListenrOnGameState(Define.GameState.End, () => SetActive(false));
        //Managers.photonGameManager.onMyCharacter += Check;
    }


    void SetActive(bool active)
    {
        _showButton.gameObject.SetActive(active);
    }

    void Check(bool isActive)
    {
        SetActive(!isActive);
        //this.gameObject.SetActive(!isActive);
        //if (Managers.Game.myPlayer == null)
        //{
        //    this.gameObject.SetActive(true);

        //}
        //else
        //{
        //    this.gameObject.SetActive(false);
        //}
    }

    void OnClick_Cancel()
    {
        SetActive(false);
    }

    void OnClick_Show()
    {
    }
}
