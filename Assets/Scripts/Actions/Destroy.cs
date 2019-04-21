using UnityEngine;

public class Destroy : Action
{
    public override void Complete()
    {
        this.gameObject.SetActive(false);
    }

    public override bool Revert()
    {
        this.gameObject.SetActive(true);
        return true;
    }

    public override bool ReDo()
    {
        this.gameObject.SetActive(false);
        return false;
    }

    private void OnDestroy()
    {
        if (this.gameObject != null && !this.gameObject.activeSelf)
            Destroy(this.gameObject);
    }
}
