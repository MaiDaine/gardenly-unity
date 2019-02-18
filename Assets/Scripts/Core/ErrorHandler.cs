using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorHandler : MonoBehaviour
{
    public static ErrorHandler instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void ErrorMessage(string msg)
    {
        /*Text errorMsg = this.GetComponent<Text>();

        errorMsg.text = msg;*/
    }
}
