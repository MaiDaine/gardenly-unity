using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientScript : MonoBehaviour
{
    public Gradient gradient;

    public Color GetColor(float gradientValue)
    {
        return gradient.Evaluate(gradientValue);
    }
}
