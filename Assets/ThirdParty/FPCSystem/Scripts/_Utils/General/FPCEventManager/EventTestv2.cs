
using UnityEngine;
using System;
using System.Collections;

public class EventTestv2 : MonoBehaviour
{
	private Action<EventParam> someListener1;
	private Action<EventParam> someListener2;
	private Action<EventParam> someListener3;

	void Awake()
	{
		someListener1 = new Action<EventParam>(SomeFunction);
		someListener2 = new Action<EventParam>(SomeOtherFunction);
		someListener3 = new Action<EventParam>(SomeThirdFunction);

		StartCoroutine(invokeTest());
	}

	IEnumerator invokeTest()
	{
		WaitForSeconds waitTime = new WaitForSeconds(0.5f);

		//Create parameter to pass to the event
		EventParam eventParam = new EventParam();
		//eventParam.sender = this.gameObject;
		eventParam.senderName = this.name;
		eventParam.recieverName = string.Empty;
		eventParam.data = null;

		while (true)
		{
			yield return waitTime;
			EventManagerv2.instance.TriggerEvent("test", eventParam);
			yield return waitTime;
			EventManagerv2.instance.TriggerEvent("Spawn", eventParam);
			yield return waitTime;
			EventManagerv2.instance.TriggerEvent("Destroy", eventParam);
		}
	}

	void OnEnable()
	{
        //Register With Action variable
        EventManagerv2.instance.StartListening("test", someListener1);
		EventManagerv2.instance.StartListening("Spawn", someListener2);
		EventManagerv2.instance.StartListening("Destroy", someListener3);

		//OR Register Directly to function
		EventManagerv2.instance.StartListening("test", SomeFunction);
		EventManagerv2.instance.StartListening("Spawn", SomeOtherFunction);
		EventManagerv2.instance.StartListening("Destroy", SomeThirdFunction);
	}

	void OnDisable()
	{
        if (EventManagerv2.IsDestroyed) return;

        //Un-Register With Action variable
        EventManagerv2.instance.StopListening("test", someListener1);
		EventManagerv2.instance.StopListening("Spawn", someListener2);
		EventManagerv2.instance.StopListening("Destroy", someListener3);

		//OR Un-Register Directly to function
		EventManagerv2.instance.StopListening("test", SomeFunction);
		EventManagerv2.instance.StopListening("Spawn", SomeOtherFunction);
		EventManagerv2.instance.StopListening("Destroy", SomeThirdFunction);
	}

	void SomeFunction(EventParam eventParam)
	{
		Debug.Log("Some Function was called!");
	}

	void SomeOtherFunction(EventParam eventParam)
	{
		Debug.Log("Some Other Function was called!");
	}

	void SomeThirdFunction(EventParam eventParam)
	{
		Debug.Log("Some Third Function was called!");
	}
}