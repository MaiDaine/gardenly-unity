﻿public interface ISerializable
{
    string Serialize();
    void DeSerialize(string json);
    void AddToSerializationNewElements();
    void AddToSerializationModifyElements();
    void AddToSerializationDeletedElements();
    SerializationController.SerializationState GetSerializationState();
}