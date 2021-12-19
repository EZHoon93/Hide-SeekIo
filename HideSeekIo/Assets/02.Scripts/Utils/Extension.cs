using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public static class Extension
{
	public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
	{
		return Util.GetOrAddComponent<T>(go);
	}

	public static Transform MyFindChild(this Transform go , string name)
    {
		return  Util.FindChild(go.gameObject, name, true).transform;
    }
	public static void BindEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
	{
		UI_Base.BindEvent(go, action, type);
	}

	public static bool IsValid(this GameObject go)
	{
		return go != null && go.activeSelf;
	}
	public static bool IsMyCharacter(this MonoBehaviourPun go)
	{
		if (IsValidAI(go.gameObject)) return false; //AI라면 False
		if (go.photonView.IsMine)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public static int ViewID(this MonoBehaviourPun go)
	{
		return go.photonView.ViewID;
	}

	public static bool IsValidAI(this GameObject go)
	{
		return go.CompareTag("AI");
	}

	public static void SetLayerRecursively(this GameObject go, int newLayer)
	{
		Util.SetLayerRecursively(go, newLayer);
	}

	public static bool CheckCreateTime(this MonoBehaviourPun go, float createTime)
    {
		return PhotonNetwork.Time <= createTime + 1 ? true : false;
	}

	public static void ResetTransform(this Transform go ,Transform parent = null, bool isScaleOne = false)
    {
        if (parent != null)
        {
			go.transform.SetParent(parent);
        }
		Util.UtillResetTransform(go );

        if (isScaleOne)
        {
			go.transform.localScale = Vector3.one;
        }
    }
}
