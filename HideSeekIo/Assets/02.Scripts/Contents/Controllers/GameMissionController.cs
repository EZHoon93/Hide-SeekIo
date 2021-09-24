using System.Collections;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class GameMissionController : MonoBehaviourPun, IPunInstantiateMagicCallback , IOnPhotonViewPreNetDestroy
{
    [SerializeField] MissionInfo[] missionInfos;
    Mission_Base _mission_Base;

    float _initRemainTime;  //미션 제한시간
    int _remainTime;
    float _createServerTime;
    bool _isPlay;

    /// <summary>
    /// 
    /// </summary>
    Define.MissionType _missionType;
    public Define.MissionType missionType
    {
        get => _missionType;
        set
        {
            Component c = GetComponent<Mission_Base>();
            if (c)
            {
                Destroy(c);
            }
            Mission_Base newMission = null;
            switch (value)
            {
                case Define.MissionType.Key:
                    newMission = this.gameObject.GetOrAddComponent<Mission_Key>();
                    break;
            }
            _mission_Base = newMission;
            _initRemainTime = _mission_Base.missionTime;
            _missionType = value;
            Managers.Game.CurrentGameScene.gameMissionController = this;
        }
    }
    public int remainTime
    {
        get => _remainTime;
        set
        {
            if (_remainTime == value) return;
            _remainTime = value;

            _mission_Base.OnUpdate(_remainTime);
            if (_remainTime <= 0)
            {
                TimeEnd();
            }

        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        missionType = (Define.MissionType)info.photonView.InstantiationData[0];
        _createServerTime = (float)info.SentServerTime;
        _isPlay = true;
        _mission_Base.OnStart(GetMissionInfo(missionType));
        print("GameMissionController OnPhotonInstantiate");
    }
    public void OnPreNetDestroy(PhotonView rootView)
    {
        _mission_Base.OnDestroy();
    }

    MissionInfo GetMissionInfo(Define.MissionType missionType)
    {
        return missionInfos.Single(s => s.missionType == missionType);
    }
     
    //void SetupMainGameScene()
    //{
    //    var gameMainScene = Managers.Game.CurrentGameScene.GetComponent<GameMainScene>();
    //    if (gameMainScene)
    //    {
    //        gameMainScene.gameMissionController = this;
    //    }
    //}

    void Update()
    {
        remainTime = (int)((_initRemainTime + _createServerTime) - (float)PhotonNetwork.Time);
    }

    //IEnumerator UpdateTime()
    //{
    //    while (true)
    //    {
    //        _mission_Base.OnUpdate();
    //        if (_currentRemainTime <= 0)
    //        {
    //            End();
    //        }
    //        yield return null;
    //    }
    //}

    void TimeEnd()
    {
        if (!_isPlay) return;
        _isPlay = false;
        _mission_Base.OnTimeEnd();
    }

   


    //void UpdateTimeUI()
    //{
    //    var time = (int)_currentRemainTime;
    //    _uI_Mission.UpdateRemainTime(time);
    //}


    //protected abstract void OnStart();
}
