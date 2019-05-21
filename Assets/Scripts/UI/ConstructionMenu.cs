using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionMenu : MonoBehaviour
{
    public Animator anim;
    public string defaultAnimation;
    public string updateAnimation;
    public bool state = false;
    public bool open = false;

    private UIController uIController = null;

    private void Start()
    {
        this.gameObject.SetActive(this.state);
        this.uIController = Camera.main.GetComponentInChildren<UIController>();
    }

    public void ChangeState(bool sleepMode = false)
    {
        if (sleepMode && !this.open)
        {
            this.open = true;
            this.gameObject.SetActive(!this.state);
        }
        if (!sleepMode)
            this.gameObject.SetActive(!this.state);
        this.state = !this.state;
    }

    public void SetState(bool state)
    {
        this.gameObject.SetActive(this.state);
        this.state = state;
    }
}
