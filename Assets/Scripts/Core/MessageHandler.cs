using UnityEngine;
using TMPro;

public class MessageHandler : MonoBehaviour
{
    public static MessageHandler instance = null;

    public Color colorRef;

    private float timer = 5;
    private TextMeshProUGUI errorMsg;
    private bool startCount = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (startCount)
            UpdateTime();
    }

    private void UpdateTime()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            errorMsg.text = "";
            startCount = false;
            timer = 5;
            gameObject.SetActive(false);
        }
    }

    public bool ErrorMessage(string msg)
    {
        msg = LocalisationController.instance.GetText("errors", msg);
        gameObject.SetActive(true);
        errorMsg = GetComponentInChildren<TextMeshProUGUI>();
        if (errorMsg != null)
        {
            errorMsg.text = msg;
            errorMsg.color = colorRef;
            startCount = true;
        }
        return false;
    }

    public bool ErrorMessage(string subCategory, string msg)
    {
        msg = LocalisationController.instance.GetText("errors", subCategory, msg);
        gameObject.SetActive(true);
        errorMsg = GetComponentInChildren<TextMeshProUGUI>();
        if (errorMsg != null)
        {
            errorMsg.text = msg;
            errorMsg.color = colorRef;
            startCount = true;
        }
        return false;
    }

    public bool SuccesMessage(string msg)
    {
        msg = LocalisationController.instance.GetText("succes", msg);
        gameObject.SetActive(true);
        errorMsg = GetComponentInChildren<TextMeshProUGUI>();
        if (errorMsg != null)
        {
            errorMsg.text = msg;
            errorMsg.color = Color.green;
            startCount = true;
        }
        return true;
    }
}
