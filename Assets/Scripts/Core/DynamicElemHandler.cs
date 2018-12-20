using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicElemHandler : GhostHandler
{

    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = 1;
    }

    //ISELECTABLE
    public override void Select()
    {
        // OPEN CANVAS WITH INFO + INTERACTION
    }

    public override void DeSelect()
    {
        // CLOSE CANVAS WITH INFO + INTERACTION
    }
}
