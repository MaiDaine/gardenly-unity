using UnityEngine;

//TODO CLEAN
public class DynamicElemHandler : DefaultStaticElement
{
    public Vector3 currentPosition;

    void Start()
    {
        this.gameObject.layer = 1;
    }
}