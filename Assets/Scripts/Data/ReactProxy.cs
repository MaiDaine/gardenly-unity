using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class ReactProxy : MonoBehaviour
{
    public static ReactProxy instance = null;

    public GraphQL graphQL;
    public ExternalData externalData;
    public Dictionary<string, Action<string>> callbacks = new Dictionary<string, Action<string>>();

    [DllImport("__Internal")]
    private static extern void save(string json);
    [DllImport("__Internal")]
    private static extern void unsavedWork(bool status);
    [DllImport("__Internal")]
    private static extern void query(string payload);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            graphQL = new GraphQL();
            externalData = new ExternalData(callbacks);
            if (Application.isEditor)
                DispatchQueryResult("{\"data\":{\"getTypes\":[{\"name\":\"Arbre\",\"id\":\"f025e92f-e115-4cbf-902b-f9551118d2a8\"},{\"name\":\"Arbuste\",\"id\":\"3741e754-c514-4715-a219-732bee92e9e7\"},{\"name\":\"Fleur\",\"id\":\"ca9c6046-38bc-4a2a-a698-0030174d8cbc\"},{\"name\":\"Legume\",\"id\":\"18d21ebd-e124-48b9-83ec-38c888257a02\"}]}}");
            else
                query(graphQL.GetPlantsTypes());
        }
        else
            Destroy(this);
    }

    //Link To REACT
    public void ExportScene()
    {
        SerializationController.instance.Serialize();
        if (Application.isEditor)
            Debug.Log(SerializationController.instance.GetSerializedData());
        else
            save(SerializationController.instance.GetSerializedData());
    }

    public void UpdateSaveState(bool state)
    {
        if (Application.isEditor)
            return;
        unsavedWork(state);
    }

    public void SendQuery(string payload)
    {
        query(payload);
    }

    //Called from REACT
    public void InitScene(string json)
    {
        if (json != "")
            SpawnController.instance.SpawnScene(SerializationController.instance.DeSerialize(json));
        if (!Application.isEditor)
            LocalisationController.instance.Init("FR");//TODO USERPREF
    }

    public void DispatchQueryResult(string json)
    {
        var jsonObject = JSONObject.Parse(json);
        if (jsonObject["errors"] != null)
        {
            Debug.Log(jsonObject["errors"].Value);//FIXME
            MessageHandler.instance.ErrorMessage("loading_error");
            return;
        }
        var keys = jsonObject["data"].Keys;
        keys.MoveNext();
        Action<string> tmp = callbacks[keys.Current.Value];
        if (tmp != null)
            tmp.Invoke(json);
    }
}
