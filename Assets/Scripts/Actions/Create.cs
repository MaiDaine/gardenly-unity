public class Create : GhostAction
{
    public override void Complete() {}

    public override bool Revert()
    {
        gameObject.SetActive(false);
        return false;
    }

    public override bool ReDo()
    {
        gameObject.SetActive(true);
        return true;
    }

    private void OnDestroy()
    {
        if (gameObject != null && !gameObject.activeSelf)
            Destroy(gameObject);
    }
}
