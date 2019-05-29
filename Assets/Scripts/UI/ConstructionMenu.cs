using UnityEngine;

// Show / Hide UI elements when click are executed
public class ConstructionMenu : MonoBehaviour
{
    public bool state = false;

    private UIController uIController = null;

    private void Start()
    {
        gameObject.SetActive(state);
        uIController = Camera.main.GetComponentInChildren<UIController>();
    }

    public void ChangeState()
    {
        gameObject.SetActive(!state);
        state = !state;
    }
}
