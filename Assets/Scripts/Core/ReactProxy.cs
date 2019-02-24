using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ReactProxy : MonoBehaviour
{
    public static ReactProxy instance = null;

    [DllImport("__Internal")]
    private static extern void UnsavedDataCheck(bool result);
    [DllImport("__Internal")]
    private static extern void SaveScene(string json, int nbElem);

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    //Link To REACT
    public void ExportScene()
    {
        SerializationController.instance.serializationJSON = SerializationController.instance.Serialize(out SerializationController.instance.serializationElemNb);
        SaveScene(SerializationController.instance.serializationJSON, SerializationController.instance.serializationElemNb);
    }

    //Called from REACT
    public void PreInitScene(int nbElem)
    {
        SerializationController.instance.serializationElemNb = nbElem;
    }

    public void InitScene(string json)
    {
        if (SerializationController.instance.serializationElemNb != 0 && json != "")
            SpawnController.instance.SpawnScene(SerializationController.instance.DeSerialize(json, SerializationController.instance.serializationElemNb));
    }

    public bool IsUnsavecWorkLeft()
    {
        //UnsavedDataCheck(true);//TODO CTRL+Z 
        return false;
    }


}
