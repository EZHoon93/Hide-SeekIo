using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

public class UI_StatController : UI_Base
{
    [SerializeField] UI_Stat[] _uI_Stats;
    [SerializeField] Transform _selectpanel;
    [SerializeField] Transform _waitPanel;
 
    bool isOn;
    readonly int selectCount = 3;   //선택될 수 
    public override void Init()
    {
        isOn = false;
        foreach(var ui in _uI_Stats)
        {
            ui.clickEvenetCallBack += () => SetActive(false);
        }
    }

    void SelectStats()
    {
        List<UI_Stat> uI_StatList = _uI_Stats.ToList();
        for(int i = 0; i < selectCount; i ++)
        {
            int ran = Random.Range(0, uI_StatList.Count);
            uI_StatList[ran].transform.ResetTransform(_selectpanel);
            uI_StatList[ran].SetActiveButton(true);
            uI_StatList.RemoveAt(ran);
        }

        foreach (var ui in uI_StatList)
            ui.transform.ResetTransform(_waitPanel);

        _selectpanel.GetComponent<HorizontalLayoutGroup>().enabled = false;
        _selectpanel.GetComponent<HorizontalLayoutGroup>().enabled = true;

    }

    public void SetActive(bool active)
    {
        //if (isOn == active) return;
        isOn = active;

        if (isOn)
        {
            SelectStats();
            _selectpanel.DOLocalMoveY(100, 0.3f).SetEase(Ease.Linear);
        }
        else
        {
            _selectpanel.DOLocalMoveY(-400, 0.3f).SetEase(Ease.Linear);
            foreach (var ui in _uI_Stats)
                ui.SetActiveButton(false);
        }

    }

}
