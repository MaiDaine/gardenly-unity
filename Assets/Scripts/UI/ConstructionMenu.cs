using UnityEngine;

// Show / Hide UI elements when click are executed
public class ConstructionMenu : MonoBehaviour
{
    public bool state = false;

    private UIController uIController = null;

    private void Start()
    {
        this.gameObject.SetActive(this.state);
        this.uIController = Camera.main.GetComponentInChildren<UIController>();
    }

    public void ChangeState()
    {
        this.gameObject.SetActive(!this.state);
        this.state = !this.state;
    }
}
