using System;
using System.Linq;
using System.Runtime.InteropServices;

using Photon.Realtime;

using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class StatSelectManager 
{
    //private readonly int initSelectSkillCount = 3;  //

    ///// <summary>
    ///// 3개 랜덤선택
    ///// </summary>
    //public Define.StatType[] RandomSelectOnlySkill()
    //{
    //    //var randomEnums =  Util.RandomEnum<Define.StatType>();
    //    var enumArray = Util.EnumToArray<Define.StatType>();
    //    //enumArray.Select
    //    var resultArray = (from e in enumArray
    //             where (int)e >= (int)Define.StatType.Stealth
    //             select e).OrderBy(g => Guid.NewGuid()).Take(initSelectSkillCount).ToArray();

    //    return resultArray;
    //}

    //public Define.Weapon[] RandomSelectOnlyWeapon()
    //{
    //    var enumArray = Util.EnumToArray<Define.Weapon>();
    //    //enumArray.Select
    //    var resultArray = (from e in enumArray
    //                       where (int)e > (int)Define.Weapon.Hammer
    //                       select e).OrderBy(g => Guid.NewGuid()).Take(initSelectSkillCount).ToArray();
    //    return resultArray;
    //}


    //public void PostEvent_StatDataToServer(PlayerController  playerController, Define.StatType newStat)
    //{
    //    if (playerController.photonView.IsMine == false) return;  //로컬 유저만 실행.
    //    var playerStat = playerController.playerStat;
    //    var copyOriginalData = playerStat.statDataList.ToList();
    //    Hashtable removeData = new Hashtable()
    //    {
    //        { "vid", playerStat.ViewID() },
    //    };
    //    copyOriginalData.Add((int)newStat);
    //    Hashtable nextHashtable = new Hashtable()
    //    {
    //        { "vid", playerStat.ViewID() },
    //        { "stl" , copyOriginalData.ToArray()}
    //    };

    //    Debug.Log("AI스킬보냄");
    //    Managers.photonGameManager.SendEvent(Define.PhotonOnEventCode.AbilityCode, EventCaching.RemoveFromRoomCache, removeData);
    //    Managers.photonGameManager.SendEvent(Define.PhotonOnEventCode.AbilityCode, EventCaching.AddToRoomCacheGlobal, nextHashtable);
    //}

    //public void OnEvent_StatDatasByServer(PlayerController playerController, int[] datas)
    //{
    //    var copyReciveData = datas.ToList();
    //    var playerStat = playerController.playerStat;
    //    //기존에있던 데이터들 제거
    //    foreach (var originalData in playerStat.statDataList)
    //    {
    //        var isExistData = playerStat.statDataList.Find(s => s == originalData);
    //        copyReciveData.Remove(isExistData);
    //    }
    //    //새로추가된 데이터 추가
    //    foreach (var addData in copyReciveData)
    //    {
    //        UpdatePlayerStatData(playerController, (Define.StatType)addData);
    //    }
    //}

    ///// <summary>
    ///// 가진 스킬을 제거하고 랜덤스ㄴ
    ///// </summary>
    ///// <param name="hasSkill"> 선택에 제외할 값</param>
    ///// <param name="statTypeArray">선택될수있는 후보, 선택제외값을 제거해야함</param>
    //public Define.StatType[] GetStatArrayExceptSkill(Define.Skill hasSkill, Define.StatType[] statTypeArray)
    //{
    //    if (statTypeArray.Length < initSelectSkillCount) return null;
    //    var selectArray = statTypeArray.Where(s => (int)s != (int)hasSkill).OrderBy(s => Guid.NewGuid()).Take(initSelectSkillCount).ToArray();
    //    return selectArray;
    //}

    //void UpdatePlayerStatData(PlayerController playerController, Define.StatType newStatDataType)
    //{
    //    var playerStat = playerController.playerStat;
    //    //스킬
    //    if ((int)newStatDataType >= 100)
    //    {
    //        var newSkill = CreateSkill(newStatDataType.ToString());
    //        newSkill.OnPhotonInstantiate(playerController);
    //    }
    //    //스탯
    //    else
    //    {

    //    }
    //    playerStat.statDataList.Add((int)newStatDataType);
    //}

    //Skill_Base CreateSkill(string skillID)
    //{
    //    var skillObject = Managers.Resource.Instantiate($"Skill/{skillID}").GetComponent<Skill_Base>();

    //    return skillObject;
    //}

    //public void SetupRandomWeapon(PlayerController playerController)
    //{
    //    if (playerController.photonView.IsMine == false) return;

    //    var selectWeaponArray = RandomSelectOnlyWeapon();
    //    //if (playerController.IsMyCharacter())
    //    //{
    //    //    var uimain = Managers.UI.SceneUI as UI_Main;
    //    //    uimain.StatController.ShowWeaponList(selectWeaponArray);
    //    //}
    //    ////AI 랜덤 무기세팅
    //    //else
    //    //{
    //    //    var ran = UnityEngine.Random.Range(0, selectWeaponArray.Length);
    //    //    var seelectWeapon = selectWeaponArray[ran];
    //    //    Managers.Spawn.WeaponSpawn(seelectWeapon, playerController);    
    //    //}
    //    if (playerController.photonView.IsMine)
    //    {
    //        var ran = UnityEngine.Random.Range(0, selectWeaponArray.Length);
    //        var seelectWeapon = selectWeaponArray[ran];
    //        Managers.Spawn.WeaponSpawn(seelectWeapon, playerController);
    //    }
    //}

    //public void SetupWeapon(PlayerController playerController , Define.Weapon weapon)
    //{
    //}
}
