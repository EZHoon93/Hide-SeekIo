using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//-----------------------------------------------------------
//Enum defining all possible game events
//More events should be added to the list
public enum EVENT_TYPE {
	
	Game_Join,
	Game_Start
	};
//-----------------------------------------------------------
//Singleton EventManagerEx to send events to listeners
//Works with IListener implementations
public class EventManagerEx : MonoBehaviour
{
	#region C# properties
	//-----------------------------------------------------------
	//Public access to instance
	public static EventManagerEx Instance
	{
		get{return instance;}
		set{}
	}
	
	#endregion

	#region variables
	//Internal reference to Notifications Manager instance (singleton design pattern)
	private static EventManagerEx instance = null;

	// Declare a delegate type for events
	public delegate void OnEvent(EVENT_TYPE Event_Type, Component Sender, object Param = null);

	//Array of listener objects (all objects registered to listen for events)
	private Dictionary<EVENT_TYPE, List<OnEvent>> Listeners = new Dictionary<EVENT_TYPE, List<OnEvent>>();
	#endregion
	//-----------------------------------------------------------
	#region methods
	//Called at start-up to initialize
	void Awake()
	{
		//If no instance exists, then assign this instance
		if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject); //Prevent object from being destroyed on scene exit
		}
		else //Instance already exists, so destroy this one. This should be a singleton object
			DestroyImmediate(this);
	}
	//-----------------------------------------------------------
	/// <summary>
	/// Function to add specified listener-object to array of listeners
	/// </summary>
	/// <param name="Event_Type">Event to Listen for</param>
	/// <param name="Listener">Object to listen for event</param>
	public void AddListener(EVENT_TYPE Event_Type, OnEvent Listener)
	{
		//List of listeners for this event
		List<OnEvent> ListenList = null;

		//New item to be added. Check for existing event type key. If one exists, add to list
		if(Listeners.TryGetValue(Event_Type, out ListenList))
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
	//-----------------------------------------------------------
	/// <summary>
	/// Function to post event to listeners
	/// </summary>
	/// <param name="Event_Type">Event to invoke</param>
	/// <param name="Sender">Object invoking event</param>
	/// <param name="Param">Optional argument</param>
	public void PostNotification(EVENT_TYPE Event_Type, Component Sender, object Param = null)
	{
		//Notify all listeners of an event

		//List of listeners for this event only
		List<OnEvent> ListenList = null;

		//If no event entry exists, then exit because there are no listeners to notify
		if(!Listeners.TryGetValue(Event_Type, out ListenList))
			return;

		//Entry exists. Now notify appropriate listeners
		for(int i=0; i<ListenList.Count; i++)
		{
			if(!ListenList[i].Equals(null)) //If object is not null, then send message via interfaces
				ListenList[i](Event_Type, Sender, Param);
		}
	}
	//-----------------------------------------------------------
	//Remove event type entry from dictionary, including all listeners
	public void RemoveEvent(EVENT_TYPE Event_Type)
	{
		//Remove entry from dictionary
		Listeners.Remove(Event_Type);
	}
	//-----------------------------------------------------------
	//Remove all redundant entries from the Dictionary
	public void RemoveRedundancies()
	{
		//Create new dictionary
		Dictionary<EVENT_TYPE, List<OnEvent>> TmpListeners = new Dictionary<EVENT_TYPE, List<OnEvent>>();
		
		//Cycle through all dictionary entries
		foreach(KeyValuePair<EVENT_TYPE, List<OnEvent>> Item in Listeners)
		{
			//Cycle through all listener objects in list, remove null objects
			for(int i = Item.Value.Count-1; i>=0; i--)
			{
				//If null, then remove item
				if(Item.Value[i].Equals(null))
					Item.Value.RemoveAt(i);
			}
			
			//If items remain in list for this notification, then add this to tmp dictionary
			if(Item.Value.Count > 0)
				TmpListeners.Add (Item.Key, Item.Value);
		}
		
		//Replace listeners object with new, optimized dictionary
		Listeners = TmpListeners;
	}

    #endregion


 //   private void Start()
 //   {
 //   EventManagerEx.Instance.AddListener(EVENT_TYPE.Game_Start, OnEvent);

	//}

	//public void OnEvent(EVENT_TYPE Event_Type, Component Sender, object Param = null)
	//{
	//	//Detect event type
	//	switch (Event_Type)
	//	{
	//		case EVENT_TYPE.Game_Start:
	//			OnGameStart(Sender, (int)Param);
	//			break;
	//	}
	//}

	//void OnGameStart(Component Enemy, int NewHealth)
	//{
	//	//If health has changed of this object
	//	if (this.GetInstanceID() != Enemy.GetInstanceID()) return;

	//	Debug.Log("Object: " + gameObject.name + " Health is: " + NewHealth.ToString());
	//}
}
