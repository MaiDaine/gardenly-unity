public class Create : GhostAction
{
    public override bool Complete()
    {
        ISerializable serializable = gameObject.GetComponent<ISerializable>();
        if (serializable != null) 
            serializable.AddToSerializationNewElements();
        return true;
    }

    public override bool Revert()
    {
        gameObject.SetActive(false);
        ISerializable serializable = gameObject.GetComponent<ISerializable>();
        if (serializable != null)
            serializable.AddToSerializationDeletedElements();
        return false;
    }

    public override bool ReDo()
    {
        gameObject.SetActive(true);
        ISerializable serializable = gameObject.GetComponent<ISerializable>();
        if (serializable != null)
            serializable.AddToSerializationNewElements();
        return true;
    }

    private void OnDestroy()
    {
        if (gameObject != null && !gameObject.activeSelf)
            Destroy(gameObject);
    }
}
