using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public enum EventType
    {
        GameManager,
        GameState,
        UIEvent,
        Camera
    }
    Dictionary<EventType, Dictionary<Enum, object[]>> listners = new Dictionary<EventType, Dictionary<Enum, object[]>>();

	//public static EventManager instacne;

    private void Awake()
    {
		Managers.eventManager = this;
    }

    public delegate void OnEvent(EventType eventType, Enum @enum, Component Sender, object Param = null);

	//Array of listener objects (all objects registered to listen for events)
	private Dictionary<EventType, List<OnEvent>> Listeners = new Dictionary<EventType, List<OnEvent>>();

	public void AddListener(EventType @evnetType, Enum @enum, object[] data)
	{
		//List of listeners for this event
		Dictionary<Enum, object[]> listnersDic = null;

		//New item to be added. Check for existing event type key. If one exists, add to list
		if (listners.TryGetValue(@evnetType, out listnersDic))
		{
			//List exists, so add new item
			//ListenList.Add(Listener);
			return;
		}

		//Otherwise create new list as dictionary key
		//ListenList = new List<OnEvent>();
		//ListenList.Add(Listener);
		//Listeners.Add(Event_Type, ListenList); //Add to internal listeners list
	}


	public void AddListener(EventType Event_Type, OnEvent Listener)
	{
		//List of listeners for this event
		List<OnEvent> ListenList = null;

		//New item to be added. Check for existing event type key. If one exists, add to list
		if (Listeners.TryGetValue(Event_Type, out ListenList))
		{
			//List exists, so add new item
			ListenList.Add(Listener);
			return;
		}

		//Otherwise create new list as dictionary key
		ListenList = new List<OnEvent>();
		ListenList.Add(Listener);
		Listeners.Add(Event_Type, ListenList); //Add to internal listeners list
	}


	public void PostNotification(EventType eventType,Enum @enum, Component Sender, object Param = null)
	{
		//Notify all listeners of an event

		//List of listeners for this event only
		List<OnEvent> ListenList = null;

		//If no event entry exists, then exit because there are no listeners to notify
		if (!Listeners.TryGetValue(eventType, out ListenList))
			return;

		//Entry exists. Now notify appropriate listeners
		for (int i = 0; i < ListenList.Count; i++)
		{
			if (!ListenList[i].Equals(null)) //If object is not null, then send message via interfaces
				ListenList[i](eventType, @enum, Sender, Param);
		}
	}
}
