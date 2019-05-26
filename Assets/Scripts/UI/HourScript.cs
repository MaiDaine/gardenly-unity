using Doozy.Engine.UI;
using TMPro;
using UnityEngine;

public class HourScript : MonoBehaviour
{
    public DayNightController dayNightController;

    private const float hourModif = 1f / 24f;

    public void UpdateHour(bool add = true)
    {
        UIButton btn = this.GetComponent<UIButton>();
        if (btn != null)
        {
            TextMeshProUGUI txt = btn.TextMeshProLabel;
            float vHour = dayNightController.targetTime;

            if (add)
                vHour = (vHour + hourModif) % 1f;
            else
            {
                vHour -= hourModif;
                if (vHour < 0)
                    vHour = 1 + vHour;
            }

            int hour = (int)(vHour * 24f);
            if (hour >= 10)
                txt.SetText("{0} : 00", hour);
            else
                txt.SetText("0{0} : 00", hour);
            this.dayNightController.SetTimeOfDay(vHour);
        }
    }
}
