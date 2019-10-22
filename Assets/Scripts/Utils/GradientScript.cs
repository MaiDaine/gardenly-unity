using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientScript : MonoBehaviour
{
    private Gradient gradient;
    private GradientColorKey[] colorKey;
    private GradientAlphaKey[] alphaKey;


    private void Start()
    {
        gradient = new Gradient();

        colorKey = new GradientColorKey[2];
        colorKey[0].color = Color.red;
        colorKey[0].time = 0.0f;
        colorKey[1].color = Color.blue;
        colorKey[1].time = 1.0f;

        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 0.0f;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);
    }

    public Color GetColor(float gradientValue)
    {
        return gradient.Evaluate(gradientValue);
    }
}
