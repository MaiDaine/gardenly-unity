using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassScript : MonoBehaviour
{
    public GameObject westButton;
    public GameObject eastButton;

#pragma warning disable 0649
    [SerializeField] private Transform LightOrientation;
    [SerializeField] private GameObject Compass;
#pragma warning restore 0649

    private bool orientationEditionActive = false;
    private bool startRotate = false;
    private float compassOrientation;

    private void LateUpdate()
    {
        if (startRotate)
            MoveCompass();
    }

    public void ToggleInterface()
    {
        orientationEditionActive = !orientationEditionActive;
        Compass.SetActive(orientationEditionActive);
        westButton.SetActive(orientationEditionActive);
        eastButton.SetActive(orientationEditionActive);
    }

    public void ClearInterface()
    {
        orientationEditionActive = false;
        startRotate = false;
        Compass.SetActive(orientationEditionActive);
        westButton.SetActive(orientationEditionActive);
        eastButton.SetActive(orientationEditionActive);
    }

    public void SetOrientation(float orientation)
    {
        compassOrientation = orientation;
    }

    public void ToggleStartRotate()
    {
        startRotate = !startRotate;
    }

    public void MoveCompass()
    {
        LightOrientation.Rotate(new Vector3(0f, compassOrientation, 0f));
        Compass.transform.Rotate(new Vector3(0f, 0f, -compassOrientation));
    }
}
