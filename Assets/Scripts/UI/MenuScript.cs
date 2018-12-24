using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public bool state = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(state);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void    ChangeState()
    {
        gameObject.SetActive(!state);
        state = !state;
    }
}
