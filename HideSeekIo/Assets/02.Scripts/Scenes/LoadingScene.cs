using System.Collections;

using TMPro;

using UnityEngine;

public class LoadingScene : BaseScene
{
    [SerializeField] TextMeshProUGUI _findText;

    string _originText;
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Lobby;
        _originText = _findText.text;
    }

    
    public override void Clear()
    {
    }

    private IEnumerator Start()
    {
        StartCoroutine(UpdateFindText());
        yield return new WaitForSeconds(2.0f);
        PhotonManager.Instance.JoinRoom();
    }

    IEnumerator UpdateFindText()
    {
        int i = 0;
        while (true)
        {
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
