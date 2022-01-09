using System.Collections;

using Photon.Pun;

using TMPro;

using UnityEngine;

public class LoadingScene : BaseScene
{
    [SerializeField] TextMeshProUGUI _findText;

    string _originText;
    bool _isLoading;
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Lobby;
        _originText = _findText.text;
        _isLoading = false;
        //PhotonNetwork.LevelLoadingProgress
    }

    
    public override void Clear()
    {
    }

    private IEnumerator Start()
    {
        StartCoroutine(UpdateFindText());
        CheckPhotonState();
        yield return new WaitForSeconds(2.0f) ;
    }

    void CheckPhotonState()
    {
        switch (Managers.PhotonManager.State)
        {
            case Define.ServerState.Connect:
                if (_isLoading) return;
                _isLoading = true;
                Managers.Scene.JoinRoom();
                break;
            case Define.ServerState.Connecting:
                break;
            case Define.ServerState.DisConnect:
                Managers.PhotonManager.Connect();
                break;
        }
     
    }

    IEnumerator UpdateFindText()
    {
        int i = 0;
        while (true)
        {
            CheckPhotonState();
            _findText.text = _originText;
            i++;
            for (int j = 0;  j < i; j++)
            {
                _findText.text += ". ";
            }
            yield return new WaitForSeconds(1.0f);
            if(i > 2)
            {
                i = 0;
            }
        }

    }
}
