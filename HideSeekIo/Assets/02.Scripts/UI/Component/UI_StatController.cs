using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

public class UI_StatController : UI_Base
{
    [SerializeField] UI_Select_Base[] _uI_Stats;
    [SerializeField] Transform _selectpanel;
    [SerializeField] Transform _waitPanel;

    List<UI_Select_Base> _statSelectList = new List<UI_Select_Base>(4);
    bool isOn;
    readonly int selectCount = 3;   //선택될 수 
    public override void Init()
    {
        isOn = false;
        foreach(var ui in _uI_Stats)
        {
            ui.transform.ResetTransform(_waitPanel);
            ui.clickEvenetCallBack += () => SetActive(ui,false);
        }
    }

    void Clear()
    {
        foreach(var ui in _statSelectList)
        {
            ui.transform.ResetTransform(_waitPanel);
        }
        _statSelectList.Clear();
    }

    public void ShowSelectList(Define.StatType[] selectList)
    {
        if (isOn) return;
        Clear();
        foreach(var u in _uI_Stats)
        {
            if (selectList.Contains(u.statType))
            {
                u.transform.ResetTransform(_selectpanel);
                u.SetActiveButton(true);
                _statSelectList.Add(u);
            }
        }

        _selectpanel.GetComponent<HorizontalLayoutGroup>().enabled = false;
        _selectpanel.GetComponent<HorizontalLayoutGroup>().enabled = true;
        _selectpanel.DOLocalMoveY(100, 0.3f).SetEase(Ease.Linear);
    }

    //void SelectStats()
    //{
    //    List<UI_Select_Base> uI_StatList = _uI_Stats.ToList();
    //    for(int i = 0; i < selectCount; i ++)
    //    {
    //        int ran = Random.Range(0, uI_StatList.Count);
    //        uI_StatList[ran].transform.ResetTransform(_selectpanel);
    //        uI_StatList[ran].SetActiveButton(true);
    //        uI_StatList.RemoveAt(ran);
    //    }

    //    foreach (var ui in uI_StatList)
    //        ui.transform.ResetTransform(_waitPanel);

    //    _selectpanel.GetComponent<HorizontalLayoutGroup>().enabled = false;
    //    _selectpanel.GetComponent<HorizontalLayoutGroup>().enabled = true;

    //}

    public void SetActive(UI_Select_Base selectUI, bool active)
    {
        //if (isOn == active) return;
        isOn = active;
        if (isOn)
        {
            _selectpanel.DOLocalMoveY(100, 0.3f).SetEase(Ease.Linear);
        }
        else
        {
            Clear();
            _selectpanel.DOLocalMoveY(-400, 0.3f).SetEase(Ease.Linear);
        }

    }

}
