using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ReactProxy : MonoBehaviour
{
    public static ReactProxy instance = null;

    [DllImport("__Internal")]
    private static extern void SaveScene(string json);
    [DllImport("__Internal")]
    private static extern void SetUnsavedWorkState(bool status);

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
            ExportScene();
    }

    //Link To REACT
    public void ExportScene()
    {
        SerializationController.instance.Serialize();
        SaveScene(SerializationController.instance.GetSerializedData());
    }

    public void UpdateSaveState(bool state)
    {
        SetUnsavedWorkState(state);
    }

    //Called from REACT
    public void InitScene(string json)
    {
       if (json != "")
           SpawnController.instance.SpawnScene(SerializationController.instance.DeSerialize(json));
    }
}
