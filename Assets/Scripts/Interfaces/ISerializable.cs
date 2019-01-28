using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerializable
{
    SerializationData Serialize();
    void DeSerialize(string json);
}
