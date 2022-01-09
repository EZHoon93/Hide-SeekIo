


using UnityEngine;
//using System;
/// <summary>
/// 
/// </summary>
/// 
public interface IListener
{
	//Notification function to be invoked on Listeners when events happen
	void OnEvent(EventDefine.EventType eventType, System.Enum @enum , Component sender, params object[] param);
}
