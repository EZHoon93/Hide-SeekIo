using System.Collections;
using System.Linq;

using TMPro;

using UnityEngine;


public class UI_CharacterStat : UI_Base
{
    //UI_CharacterView _uI_CharacterView;

    enum Texts
    {
        InfoText
    }
    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        GetComponent<UI_CharacterView>().changeViewCallBack += UpdateInfo;
    }

    public void UpdateInfo(Define.CharacterType characterType)
    {
        var info = UISetting.Instance.charcterStatInfo.Single(s => s.characterType == characterType);
        GetText((int)Texts.InfoText).text = info.info;
    }
}
