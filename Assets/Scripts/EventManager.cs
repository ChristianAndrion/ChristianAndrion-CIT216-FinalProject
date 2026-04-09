using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

//This is going to replace notification manager class
//Be sure to attach this to our GameManager class!
public class EventManager : MonoBehaviour
{

    //These are the things we are going to listen to
    private Dictionary<string, UnityAction<int>> eventDictionary;

    //allows us to access the instance from other classes!
    private static EventManager eventManager;

    //using our C# setter and getter technique to setup our instance
    public static EventManager instance
    {
        get //a different way of implementing our singleton
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityAction<int>>();
        }
    }

    public static void StartListening(string eventName, UnityAction<int> listener)
    {
        if (instance.eventDictionary.ContainsKey(eventName))
        {
            instance.eventDictionary[eventName] += listener;
        }
        else
        {
            instance.eventDictionary.Add(eventName, listener);
        }
    }

    public static void StopListening(string eventName, UnityAction<int> listener)
    {
        if (instance.eventDictionary.ContainsKey(eventName))
        {
            instance.eventDictionary[eventName] -= listener;
        }
    }

    public static void TriggerEvent(string eventName, int value = 0)
    {
        UnityAction<int> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(value);
        }
    }
}