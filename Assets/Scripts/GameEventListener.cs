using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public GameEvent gameEvent;
    public UnityEvent response;

    private void OnEnable()
    {
        this.gameEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        this.gameEvent.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        this.response.Invoke();
    }
}