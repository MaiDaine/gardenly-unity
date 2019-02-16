using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorHandler : MonoBehaviour
{
    public static ErrorHandler instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void ErrorMessage(string msg)
    {
        //TODO : interface
        Debug.Log(msg);
    }
}
