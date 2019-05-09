using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create : GhostAction
{
    public override void Complete() {}

    public override bool Revert()
    {
        this.gameObject.SetActive(false);
        return false;
    }

    public override bool ReDo()
    {
        this.gameObject.SetActive(true);
        return true;
    }

    private void OnDestroy()
    {
        if (this.gameObject != null && !this.gameObject.activeSelf)
            Destroy(this.gameObject);
    }
}
