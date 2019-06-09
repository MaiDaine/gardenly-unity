public interface ISerializable
{
    string Serialize();
    void DeSerialize(string json);
}