using Doozy.Engine.UI;
using TMPro;
using UnityEngine;

public class HourScript : MonoBehaviour
{
    public DayNightController dayNightController;

    private void Start()
    {
        dayNightController.SetTimeOfDay(14);
    }

    public void UpdateHour(bool add = true)
    {
        UIButton btn = GetComponent<UIButton>();
        if (btn != null)
        {
            TextMeshProUGUI txt = btn.TextMeshProLabel;
            int hour = (int)(dayNightController.targetTime * 24f);
            
            if (add)
                hour = (hour + 1) % 24;
            else
            {
                hour -= 1;
                if (hour < 0)
                    hour = 23;
            }

            if (hour >= 10)
                txt.SetText("{0} : 00", hour);
            else
                txt.SetText("0{0} : 00", hour);
            dayNightController.SetTimeOfDay(hour);
           // dayNightController.RotateSun(add);
        }
    }
}