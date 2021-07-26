using System.Collections;

using Data;

using UnityEngine;

public static class StoreManager 
{


    //랜덤 스킨 갖고옴 , 현재 선택된 인덱스를 교체
    public static string ChangeSkin(int selectIndex)
    {
        var currentAvater = PlayerInfo.userData.skinList[selectIndex].avaterSeverKey;   //중복제거를위해 현재아바타 ID를 갖고온다
        var avaterList =  Resources.LoadAll<Animator>("Prefabs/Avater");
        string selectName = null;
        do
        {
            int ran = Random.Range(0, avaterList.Length);
            selectName = avaterList[ran].name;
            Debug.Log(selectName);
        } while (currentAvater.Equals(selectName)); //같은것을 뽑으면 다시

        PlayerInfo.userData.skinList[selectIndex].avaterSeverKey = selectName;
        PlayerInfo.SaveUserData();

        return selectName;  //선택된 아바타 아이디
    }

    public static string ChangeWeapon(int selectIndex)
    {
        var currentWeapon = PlayerInfo.userData.skinList[selectIndex].weaponSeverKey;

        var weaponList = Resources.LoadAll<GameObject>("Prefabs/Melee2");
        string selectName = null;
        do
        {
            int ran = Random.Range(0, weaponList.Length);
            selectName = weaponList[ran].name;
            Debug.Log(selectName);
        } while (currentWeapon.Equals(selectName)); //같은것을 뽑으면 다시

        PlayerInfo.userData.skinList[selectIndex].weaponSeverKey = selectName;
        PlayerInfo.SaveUserData();

        return selectName;
    }

    public static void AddSkinList()
    {
        var avaterList = Resources.LoadAll<Animator>("Prefabs/Avater");
        var weaponList = Resources.LoadAll<GameObject>("Prefabs/Melee2");

        int avaterRan = Random.Range(0, avaterList.Length);
        int weaponRan = Random.Range(0, weaponList.Length);

        PlayerInfo.userData.skinList.Add(new ServerKey(avaterList[avaterRan].name,weaponList[weaponRan].name,false));   //=>캐릭및 스킨 랜덤
        PlayerInfo.SaveUserData();

    }

    public static void ChangeNickName(string nickName)
    {

    }
}
