using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
   
    //Dictionary<Define.EventType, Dictionary<Enum, object[]>> listners = new Dictionary<Define.EventType, Dictionary<Enum, object[]>>();

	//public static EventManager instacne;

    private void Awake()
    {
		Managers.eventManager = this;

    }

    //public delegate void OnEvent(Define.EventType eventType, Enum @enum, Component Sender, object Param = null);
	private Dictionary<Define.EventType , List<IListener>> _listenerDic = new Dictionary<Define.EventType, List<IListener>>();

	
	

	public void AddListener(Define.EventType Event_Type, IListener Listener)
	{
		List<IListener> listenList = null;

		if (_listenerDic.TryGetValue(Event_Type, out listenList))
		{
			//List exists, so add new item
			listenList.Add(Listener);
			return;
		}

		//Otherwise create new list as dictionary key
		listenList = new List<IListener>();
		listenList.Add(Listener);
		_listenerDic.Add(Event_Type, listenList); //Add to internal listeners list
	}


	public void PostNotification(Define.EventType eventType,Enum @enum, Component sender, params object[] param )
	{
		List<IListener> listenList = null;

		if (!_listenerDic.TryGetValue(eventType, out listenList))
			return;

		//Entry exists. Now notify appropriate listeners
		for (int i = 0; i < listenList.Count; i++)
		{
			if (!listenList[i].Equals(null)) //If object is not null, then send message via interfaces
				listenList[i].OnEvent(eventType,@enum, sender, param);
		}
	}
}
