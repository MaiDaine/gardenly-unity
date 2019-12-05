using UnityEngine;

public class DayNightController : MonoBehaviour
{

    [Range(0, 1)]
    public float targetTime = 0.6246667f;
    [HideInInspector]
    public float timeMultiplier = 1f;

#pragma warning disable 0649
    [SerializeField] private Light sun;
    [SerializeField] private Transform sunLocation;
#pragma warning restore 0649

    private int timeAnimation;
    private float currentTimeOfDay;
    private float sunInitialIntensity;

    private const float vHour = 1f / 24f;
    private const float initialSunLocation = 150f;
    private const float sunRotateCoef = 15f;
    private bool orientationEditionActive = false;

    private void Start()
    {
        sunInitialIntensity = sun.intensity;
        currentTimeOfDay = targetTime;
        sunLocation.transform.localRotation = Quaternion.Euler(0, initialSunLocation, 0);
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

        if (currentTimeOfDay < 0.5f)
            sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 180f), 0, 0);
        else
            sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 180f), 180, 0);

        if (currentTimeOfDay < 0.25f || currentTimeOfDay > 0.75f)
            intensityMultiplier = 0;
        else
            intensityMultiplier = 1f - Mathf.Abs(0.5f - currentTimeOfDay);
        sun.intensity = sunInitialIntensity * intensityMultiplier;
    }

    public void RotateSun(bool add)
    {
        float orientation = 1;

        if (add)
            orientation = -1;
        sunLocation.transform.Rotate(new Vector3(0, sunRotateCoef * orientation, 0));
    }
}