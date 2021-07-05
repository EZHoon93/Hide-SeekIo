using System.Collections;

using UnityEngine;

public static class StoreManager 
{


    //랜덤 스킨 갖고옴
    public static void ChangeSkin()
    {
        var currentAvater = PlayerInfo.CurrentAvater;

        var avaterList =  Resources.LoadAll<Animator>("Prefabs/Avater");
        Debug.LogError(avaterList.Length + "아바타 수 ");
        string selectName = null;
        do
        {
            int ran = Random.Range(0, avaterList.Length);
            selectName = avaterList[ran].name;
            Debug.Log(selectName);
        } while (currentAvater.Equals(selectName)); //같은것을 뽑으면 다시

        PlayerInfo.userData.HasAvaters[0] = selectName;
        PlayerInfo.SaveUserData();
    }

    public static void ChangeWeapon()
    {
        var currentWeapon = PlayerInfo.CurrentAvater;

        var weaponList = Resources.LoadAll<GameObject>("Prefabs/Melee2");
        string selectName = null;
        do
        {
            int ran = Random.Range(0, weaponList.Length);
            selectName = weaponList[ran].name;
            Debug.Log(selectName);
        } while (currentWeapon.Equals(selectName)); //같은것을 뽑으면 다시

        PlayerInfo.userData.HasWeapons[0] = selectName;
        PlayerInfo.SaveUserData();
    }
}
