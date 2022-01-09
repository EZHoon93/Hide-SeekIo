using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager 
{
   
    //Dictionary<Define.EventType, Dictionary<Enum, object[]>> listners = new Dictionary<Define.EventType, Dictionary<Enum, object[]>>();

	//public static EventManager instacne;

  
	//public delegate void OnEvent(EventDefine.EventType eventType, Enum @enum, Component Sender, object Param = null);
	private Dictionary<EventDefine.EventType, List<IListener>> _listenerDic = new Dictionary<EventDefine.EventType, List<IListener>>();

	public Dictionary<EventDefine.EventType, List<IListener>> tt => _listenerDic;

	public void AddListener(EventDefine.EventType eventType, IListener Listener)
	{

	
		List<IListener> listenList = null;

		if (_listenerDic.TryGetValue(eventType, out listenList))
		{
			//List exists, so add new item
			if (eventType == EventDefine.EventType.InGame)
			{
				Debug.Log(_listenerDic[EventDefine.EventType.InGame].Count);
			}
			listenList.Add(Listener);
			return;
		}

		//Otherwise create new list as dictionary key
		listenList = new List<IListener>();
		listenList.Add(Listener);
		_listenerDic.Add(eventType, listenList); //Add to internal listeners list

	
	}


	public void PostNotification(EventDefine.EventType eventType,Enum @enum, Component sender, params object[] param )
	{
		List<IListener> listenList = null;

		if (!_listenerDic.TryGetValue(eventType, out listenList))
			return;

		//리슨한테보냄
		for (int i = 0; i < listenList.Count; i++)
		{
			if (!listenList[i].Equals(null))
            {
				listenList[i].OnEvent(eventType, @enum, sender, param);
			}
		}
	}

	//이벤트 자기자신 리슨제거
	public void RemoveEvent(EventDefine.EventType eventType, IListener listener)
	{
		//Remove entry from dictionary
		_listenerDic.Remove(eventType);

		if(_listenerDic.ContainsKey(eventType) == false)
        {
			return;
        }

		_listenerDic[eventType].Remove(listener);
	}


	//이벤트 제거
	public void RemoveEventAll(EventDefine.EventType eventType )
	{
		_listenerDic.Remove(eventType);
	}
	//이벤트 중복제거
	public void RemoveRedundancies()
	{
		//Create new dictionary
		Dictionary<EventDefine.EventType, List<IListener>> TmpListeners = new Dictionary<EventDefine.EventType, List<IListener>>();

		//Cycle through all dictionary entries
		foreach (KeyValuePair<EventDefine.EventType, List<IListener>> Item in _listenerDic)
		{
			//Cycle through all listener objects in list, remove null objects
			for (int i = Item.Value.Count - 1; i >= 0; i--)
			{
				//If null, then remove item
				if (Item.Value[i].Equals(null))
					Item.Value.RemoveAt(i);
			}

			//If items remain in list for this notification, then add this to tmp dictionary
			if (Item.Value.Count > 0)
				TmpListeners.Add(Item.Key, Item.Value);
		}

		//Replace listeners object with new, optimized dictionary
		_listenerDic = TmpListeners;
	}

	//씬 호출시 
	public void Clear()
	{
		RemoveRedundancies();
	}


}
