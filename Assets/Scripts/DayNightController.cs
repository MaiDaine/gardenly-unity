using UnityEngine;

public class DayNightController : MonoBehaviour
{
    public Light sun;
    public float secondsInFullDay = 120f;
    [Range(0, 1)]
    public float targetTime = 0.6246667f;
    [HideInInspector]
    public float timeMultiplier = 1f;

    private int timeAnimation;
    private float currentTimeOfDay;
    private float sunInitialIntensity;

    void Start()
    {
        sunInitialIntensity = sun.intensity;
        currentTimeOfDay = targetTime;
    }

    public void SetTimeOfDay(float time)
    {
        targetTime = time;
        timeAnimation = (targetTime > currentTimeOfDay) ? 1 : -1;
    }

    private void Update()
    {
        if (timeAnimation != 0)
        {
            currentTimeOfDay += (timeAnimation * Time.deltaTime) / 10f;
            if ((timeAnimation > 0 && currentTimeOfDay > targetTime)
                || (timeAnimation < 0 && currentTimeOfDay < targetTime))
            {
                currentTimeOfDay = targetTime;
                timeAnimation = 0;
            }
            UpdateSun();
        }
    }

    void UpdateSun()
    {
        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 180, 0);

        float intensityMultiplier = 1;
        if (currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.75f)
        {
            intensityMultiplier = 0;
        }
        else if (currentTimeOfDay <= 0.25f)
        {
            intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f));
        }
        else if (currentTimeOfDay >= 0.73f)
        {
            intensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.73f) * (1 / 0.02f)));
        }

        sun.intensity = sunInitialIntensity * intensityMultiplier;
    }
}