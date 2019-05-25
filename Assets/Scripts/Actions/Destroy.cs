public class Destroy : GhostAction
{
    public override void Complete()
    {
        gameObject.SetActive(false);
    }

    public override bool Revert()
    {
        gameObject.SetActive(true);
        return true;
    }

    public override bool ReDo()
    {
        gameObject.SetActive(false);
        return false;
    }

    private void OnDestroy()
    {
        if (gameObject != null && !gameObject.activeSelf)
            Destroy(gameObject);
    }
}
