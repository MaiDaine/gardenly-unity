using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageHandler : MonoBehaviour
{
    public static MessageHandler instance = null;

    private float timer = 5;
    private Text errorMsg;
    private bool startCount = false;

    private void Awake()
    {
        instance = this;
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (this.startCount)
            UpdateTime();
    }

    private void UpdateTime()
    {
        this.timer -= Time.deltaTime;

        if (this.timer <= 0)
        {
            this.errorMsg.text = "";
            this.startCount = false;
            this.timer = 5;
            this.gameObject.SetActive(false);
        }
    }

    public bool ErrorMessage(string msg)
    {
        msg = LocalisationController.instance.GetText("errors", msg);
        this.gameObject.SetActive(true);
        this.errorMsg = this.GetComponentInChildren<Text>();
        if (this.errorMsg != null)
        {
            this.errorMsg.text = msg;
            this.startCount = true;
        }
        return false;
    }

    public bool ErrorMessage(string subCategory, string msg)
    {
        msg = LocalisationController.instance.GetText("errors", subCategory, msg);
        this.gameObject.SetActive(true);
        this.errorMsg = this.GetComponentInChildren<Text>();
        if (this.errorMsg != null)
        {
            this.errorMsg.text = msg;
            this.startCount = true;
        }
        return false;
    }

    public bool SuccesMessage(string msg)
    {
        msg = LocalisationController.instance.GetText("succes", msg);
        this.gameObject.SetActive(true);
        this.errorMsg = this.GetComponentInChildren<Text>();
        if (this.errorMsg != null)
        {
            this.errorMsg.text = msg;
            this.errorMsg.color = Color.green;
            this.startCount = true;
        }
        return true;
    }
}
