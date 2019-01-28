using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultStaticElement : GhostHandler
{
    public Vector3 correctedRotation;

    private void Start()
    {
        this.transform.eulerAngles += correctedRotation;
    }
}
