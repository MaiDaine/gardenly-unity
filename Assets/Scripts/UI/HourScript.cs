using Doozy.Engine.UI;
using TMPro;
using UnityEngine;

/*
    C'est logiquement incorrect selon moi.
    L'UI ne devrait pas avoir à faire des calculs dans du display

    Pour moi:

        DayNightController implements :
            - getTime() -> return heure de 0 - 24
            - incHour(int cnt = 1) -> return nouvelle heure si succès de 0 - 24 sinon ancienne heure
            - decHour(int cnt = 1) -> return nouvelle heure si succès de 0 - 24 sinon ancienne heure

        HourScript (Que je renomme UI_DayNight_XXX) implements :
            - UpdateHour(bool add = true, int qty = 1) {
                UIButton btn = GetComponent<UIButton>();

                if (btn) {
                    TextMeshProUGUI txt = btn.TextMeshProLabel;
                    int newTime = 0;

                    if (add)
                        newTime = dayNightController.incHour(qty);
                    else
                        newTime = dayNightController.decHour(qty);
                    
                    if (newTime >= 10)
                        txt.SetText("{0} : 00", newTime);
                    else
                        txt.SetText("0{0} : 00", newTime);
                }
            }
 */

public class HourScript : MonoBehaviour
{
    public DayNightController dayNightController;

    private const float hourModif = 1f / 24f;

    public void UpdateHour(bool add = true)
    {
        UIButton btn = GetComponent<UIButton>();
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
            dayNightController.SetTimeOfDay(vHour);
        }
    }
}
