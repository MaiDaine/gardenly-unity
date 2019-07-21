using UnityEngine;

// Show / Hide UI elements when click are executed
public class ConstructionMenu : MonoBehaviour
{
    public bool state = false;

    private void Start()
    {
        gameObject.SetActive(state);
    }

    public void ChangeState()
    {
        state = !state;
        gameObject.SetActive(state);
    }
}
