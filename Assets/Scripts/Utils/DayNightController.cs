using UnityEngine;

public class DayNightController : MonoBehaviour
{

    [Range(0, 1)]
    public float targetTime = 0.6246667f;
    [HideInInspector]
    public float timeMultiplier = 1f;

#pragma warning disable 0649
    [SerializeField] private Light sun;
    [SerializeField] private Transform sun2;
#pragma warning restore 0649

    private int timeAnimation;
    private float currentTimeOfDay;
    private float sunInitialIntensity;

    private const float vHour = 1f / 24f;
    private bool orientationEditionActive = false;

    private void Start()
    {
        sunInitialIntensity = sun.intensity;
        currentTimeOfDay = targetTime;
    }

    public void SetTimeOfDay(float time)
    {
        if (time == 0 || time == 23)
        {
            InstantSetTimeofDay(time);
            return;
        }
        targetTime = time * vHour;
        timeAnimation = (targetTime > currentTimeOfDay) ? 1 : -1;
    }

    public void InstantSetTimeofDay(float time)
    {
        time *= vHour;
        targetTime = time;
        currentTimeOfDay = time;
        UpdateSun();
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
        float intensityMultiplier = 1;

        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 180f), 0, 0);
        if (currentTimeOfDay < 0.5f)
            sun2.transform.localRotation = Quaternion.Euler(0.0122f, -(currentTimeOfDay * 360f), -0.171f);
        else
            sun2.transform.localRotation = Quaternion.Euler(0.0122f, (currentTimeOfDay * 360f), -0.171f);

        if (currentTimeOfDay < 0.25f || currentTimeOfDay > 0.75f)
            intensityMultiplier = 0;
        else
            intensityMultiplier = 1f - Mathf.Abs(0.5f - currentTimeOfDay);
        sun.intensity = sunInitialIntensity * intensityMultiplier;
    }
}