using UnityEngine;

public class DayNightController : MonoBehaviour
{

    [Range(0, 1)]
    public float targetTime = 0.6246667f;
    [HideInInspector]
    public float timeMultiplier = 1f;

#pragma warning disable 0649
    [SerializeField] private Light sun;
    [SerializeField] private Transform LightOrientation;
    [SerializeField] private GameObject VisualOrientation;
    [SerializeField] private GameObject Compass;
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

    private void LateUpdate()
    {
        //TODO UI
        if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            orientationEditionActive = !orientationEditionActive;
            VisualOrientation.SetActive(orientationEditionActive);
            Compass.SetActive(orientationEditionActive);
        }
        if (orientationEditionActive)
        {
            float rotx = Input.GetAxis("Mouse X") * 100f * Mathf.Deg2Rad;
            Compass.transform.Rotate(Vector3.forward, -rotx);
            LightOrientation.Rotate(new Vector3(0f, rotx, 0f));

            if (Input.GetKey(KeyCode.KeypadPlus))
            {
                LightOrientation.Rotate(new Vector3(0f, 1f, 0f));
                Compass.transform.Rotate(new Vector3(0f, 0f, -1f));
            }
            else if (Input.GetKey(KeyCode.KeypadMinus))
            {
                LightOrientation.Rotate(new Vector3(0f, -1f, 0f));
                Compass.transform.Rotate(new Vector3(0f, 0f, 1f));
            }
        }
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
        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 180f), 0, 0);

        float intensityMultiplier = 1;
        if (currentTimeOfDay < 0.25f || currentTimeOfDay > 0.75f)
            intensityMultiplier = 0;
        else
            intensityMultiplier = 1f - Mathf.Abs(0.5f - currentTimeOfDay);
        sun.intensity = sunInitialIntensity * intensityMultiplier;
    }
}