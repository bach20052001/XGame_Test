using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDispatcher : MonoBehaviour
{
    #region Singleton

    private static EventDispatcher instance;

    public static EventDispatcher Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject eventDispatch = new GameObject();
                Instantiate(eventDispatch);
                EventDispatcher _instance = eventDispatch.AddComponent<EventDispatcher>();
                instance = _instance;
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance != null && EventDispatcher.Instance != instance)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Register, Post, Remove
    Dictionary<Event, Action<object>> listener = new Dictionary<Event, Action<object>>();

    public void RegisterListener(Event eventID, Action<object> callback)
    {
        if (listener.ContainsKey(eventID))
        {
            listener[eventID] += callback;
        }
        else
        {
            listener.Add(eventID, null);
            listener[eventID] += callback;
        }
    }
    public void PostEvent(Event eventID)
    {
        var callback = listener[eventID];

        if (callback != null)
        {
            callback(null);
        }
        else
        {
            Debug.LogError("Can't find callback function of" + eventID.ToString());
        }
    }
    public void PostEvent(Event eventID, object obj)
    {
        var callback = listener[eventID];

        if (callback != null)
        {
            callback(obj);
        }
        else
        {
            Debug.LogError("Can't find callback function of" + eventID.ToString());
        }
    }

    public void RemoveListener(Event eventID, Action<object> callback)
    {
        if (listener.ContainsKey(eventID))
        {
            listener[eventID] -= callback;
        }
    }

    #endregion

}
#region Extension Method

public static class EventDispatcherExtension
{
    public static void RegisterListener(this MonoBehaviour listener, Event eventID, Action<object> callback)
    {
        EventDispatcher.Instance.RegisterListener(eventID, callback);
    }

    public static void PostEvent(this MonoBehaviour listener, Event eventID, object param)
    {
        EventDispatcher.Instance.PostEvent(eventID, param);
    }

    public static void PostEvent(this MonoBehaviour listener, Event eventID)
    {
        EventDispatcher.Instance.PostEvent(eventID);
    }

    public static void RemoveListener(this MonoBehaviour listener, Event eventID, Action<object> callback)
    {
        EventDispatcher.Instance.RemoveListener(eventID, callback);
    }
}

#endregion