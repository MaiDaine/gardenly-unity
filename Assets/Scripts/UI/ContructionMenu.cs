using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContructionMenu : MonoBehaviour
{ 
    public bool state = false;

    void Start()
    {
        gameObject.SetActive(state);
    }

    public void ChangeState()
    {
        gameObject.SetActive(!state);
        state = !state;
    }
}
