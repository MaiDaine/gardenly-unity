using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorHandler : MonoBehaviour
{
    public static ErrorHandler instance = null;

    private float timer = 5;
    private Text errorMsg;
    private bool startCount = false;

    void Awake()
    {
        instance = this;
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        if (startCount)
            UpdateTime();
    }

    void UpdateTime()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            errorMsg.text = "";
            startCount = false;
            timer = 5;
            this.gameObject.SetActive(false);
        }
    }

    public void ErrorMessage(string msg)
    {
        this.gameObject.SetActive(true);
        errorMsg = this.GetComponentInChildren<Text>();
        if (errorMsg != null)
        {
            errorMsg.text = msg;
            startCount = true;
        }
    }
}
