public class Destroy : GhostAction
{
    public override bool Complete()
    {
        gameObject.SetActive(false);
        return true;
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
