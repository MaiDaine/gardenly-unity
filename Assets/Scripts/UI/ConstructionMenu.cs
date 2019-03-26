using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionMenu : MonoBehaviour
{
    public bool state = false;

    private void Start()
    {
        this.gameObject.SetActive(this.state);
    }

    private void ChangeState()
    {
        this.gameObject.SetActive(!this.state);
        this.state = !this.state;
    }


    public void SetState(bool state)
    {
        this.gameObject.SetActive(this.state);
        this.state = state;
    }
}
