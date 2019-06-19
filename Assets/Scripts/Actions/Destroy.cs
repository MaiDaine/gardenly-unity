public class Destroy : GhostAction
{
    public override bool Complete()
    {
        gameObject.SetActive(false);
        ISerializable serializable = gameObject.GetComponent<ISerializable>();
        if (serializable != null)
            serializable.AddToSerializationDeletedElements();
        return true;
    }

    public override bool Revert()
    {
        gameObject.SetActive(true);
        ISerializable serializable = gameObject.GetComponent<ISerializable>();
        if (serializable != null)
            serializable.AddToSerializationModifyElements();
        return true;
    }

    public override bool ReDo()
    {
        gameObject.SetActive(false);
        ISerializable serializable = gameObject.GetComponent<ISerializable>();
        if (serializable != null)
            serializable.AddToSerializationDeletedElements();
        return false;
    }

    private void OnDestroy()
    {
        if (gameObject != null && !gameObject.activeSelf)
            Destroy(gameObject);
    }
}