using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Track : Skill_Base
{

    public override Define.ControllerType controllerType { get; set; } = Define.ControllerType.Button;

    private void Start()
    {
        InitCoolTime = 5;
    }

    public override void Use(PlayerController usePlayerController)
    {
        print("트랙");
        var allHiderList = Managers.Game.GetAllHiderList();
        float minDistance = 9999;
        Transform minTarget = null;
        //foreach(var hider in allHiderList)
        //{
        //   var distance = Vector3.Distance(usePlayerController.transform.position, hider.transform.position);
        //   if(distance < minDistance)
        //    {
        //        minTarget = hider.transform;
        //    }
        //}
        //if (minTarget == null) return;

        var trackItem = Managers.Resource.Instantiate("Contents/TrackItem").GetComponent<TrackItem>();
        trackItem.transform.position = usePlayerController.transform.position;
        trackItem.SetupTarget(Managers.Game.CurrentGameScene.test);
    }
}
