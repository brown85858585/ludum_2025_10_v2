

//====================================================================================================
// Example use:
//
// Send a Message --> EventManagerv2.instance.TriggerEvent("Test");
//                    EventManagerv2.instance.TriggerEvent("Test", new EventParam(this.name, string.Empty, ""));
//
// Listen to recieve a message --> EventManagerv2.instance.StartListening("Test", SomeListener);
// Stop Listening to a message --> EventManagerv2.instance.StopListening("Test", SomeListener);
//
// SomeListener is the function that will be executed when the message is recieved.
// private void SomeListener(EventParam eventParam) { }
//
//
// NOTE: If you don't include an EventManager in your scene but you are using it in your code, 
// the EventManeger itself will be created in your scene in Runtime.
//
//====================================================================================================



using System;
using System.Collections.Generic;


//Re-usable structure/ Can be a class to. Add all parameters you need inside it
public struct EventParam
{
	// Add event parameters here
	public String senderName;
	public String recieverName;
    public object data;

    // Constructor (not necessary, but helpful)
    public EventParam(string _sender, string _reciever, object _data)
    {
        senderName = _sender;
        recieverName = _reciever;
        data = _data;
    }
}


public class EventManagerv2 : Singletone<EventManagerv2>
{

	private Dictionary<string, Action<EventParam>> eventDictionary;

	/*private static EventManagerv2 eventManagerv2;

	public static EventManagerv2 instance
	{
		get
		{
			if (!eventManagerv2)
			{
				eventManagerv2 = FindObjectOfType<EventManagerv2>();

				if (!eventManagerv2)
				{
					Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
				}
				else
				{
					eventManagerv2.Init();
				}
			}
			return eventManagerv2;
		}
	}*/

	private void Init()
	{
		if (eventDictionary == null)
		{
			eventDictionary = new Dictionary<string, Action<EventParam>>();
		}
	}

    void Start ()
    {
        Init();
    }

    public void StartListening(string eventName, Action<EventParam> listener)
	{
        if (eventDictionary == null) Init();    // Security Sentence

        Action<EventParam> thisEvent;
		if (eventDictionary.TryGetValue(eventName, out thisEvent))
		{
			//Add more event to the existing one
			thisEvent += listener;

            //Update the Dictionary
            eventDictionary[eventName] = thisEvent;
		}
		else
		{
			//Add event to the Dictionary for the first time
			thisEvent += listener;
            eventDictionary.Add(eventName, thisEvent);
		}
	}

	public void StopListening(string eventName, Action<EventParam> listener)
	{
        Action<EventParam> thisEvent;
		if (eventDictionary.TryGetValue(eventName, out thisEvent))
		{
			//Remove event from the existing one
			thisEvent -= listener;

			//Update the Dictionary
			eventDictionary[eventName] = thisEvent;
		}
	}

    public void TriggerEvent(string eventName)
    {
        TriggerEvent(eventName, new EventParam(this.name, string.Empty, ""));
    }

    public void TriggerEvent(string eventName, EventParam eventParam)
	{
		Action<EventParam> thisEvent = null;
		if (eventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.Invoke(eventParam);
			// OR USE  instance.eventDictionary[eventName](eventParam);
		}
	}
}