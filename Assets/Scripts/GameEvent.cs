using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    private readonly List<GEventListener> eventListeners =
        new List<GEventListener>();

    public void Raise()
    {
        for (int i = this.eventListeners.Count - 1; i >= 0; i--)
            this.eventListeners[i].OnEventRaised();
    }

    public void RegisterListener(GEventListener listener)
    {
        if (!this.eventListeners.Contains(listener))
            this.eventListeners.Add(listener);
    }

    public void UnregisterListener(GEventListener listener)
    {
        if (this.eventListeners.Contains(listener))
            this.eventListeners.Remove(listener);
    }
}
