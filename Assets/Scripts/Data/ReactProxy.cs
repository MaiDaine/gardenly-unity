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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
            ExportScene();
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            if (Application.isEditor)
                DispatchQueryResult("{\"data\":{\"getAllPlants\":{\"Fleur\":[{\"node\":{\"name\":\"Abricotier\"}},{\"node\":{\"name\":\"Campanule\"}},{\"node\":{\"name\":\"Capucine\"}},{\"node\":{\"name\":\"Coquelicot\"}},{\"node\":{\"name\":\"Crocus\"}},{\"node\":{\"name\":\"Edelweiss\"}},{\"node\":{\"name\":\"Gardénia\"}},{\"node\":{\"name\":\"Jacinthe\"}},{\"node\":{\"name\":\"Lys\"}},{\"node\":{\"name\":\"Narcisse\"}},{\"node\":{\"name\":\"Œillet\"}},{\"node\":{\"name\":\"Oeillet d'Inde\"}},{\"node\":{\"name\":\"Orchidées\"}},{\"node\":{\"name\":\"Pensée\"}},{\"node\":{\"name\":\"Pétunia\"}},{\"node\":{\"name\":\"Phalaenopsis\"}},{\"node\":{\"name\":\"Pivoine\"}}]}}}");
            else
                query(graphQL.GetPlantsOfType("Fleur", externalData.plantsTypes["Fleur"]));
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            if (Application.isEditor)
                DispatchQueryResult("{\"data\":{\"getPlant\":{\"name\":\"Pétunia\",\"blossomingEnd\":[4],\"blossomingStart\":[2],\"color\":[\"Vert\",\"Rose\"],\"createdAt\":\"Sat, 04 May 2019 23:45:58 +0000\",\"groundTypes\":[{\"id\":\"e060619a-7977-4280-ba21-f5673ebb6817\"},{\"id\":\"e09f04bf-57a0-45cc-8e89-00f3657bd287\"},{\"id\":\"f6ddd7f8-329e-4809-a869-40281392823b\"}],\"heightLow\":10000,\"heightHigh\":null,\"id\":\"6bcc1cc7-32dd-4e38-b516-7b70d341707f\",\"periodicities\":[{\"name\":\"Vivace\"}],\"phRangeLow\":7,\"phRangeHigh\":7,\"photo\":\"https://s3.greefine.ovh/dev/8cd324ca744800af98885ec7378bf5ad390fe3ea/e57ce8f2-a9b8-4de8-94a7-d7efea5ccffd.jpg\",\"rusticity\":5,\"shapes\":[{\"name\":\"Arrondi\"}],\"sunNeed\":8,\"thumbnail\":\"https://s3.greefine.ovh/dev/8cd324ca744800af98885ec7378bf5ad390fe3ea/thumbnail_e57ce8f2-a9b8-4de8-94a7-d7efea5ccffd.jpg\",\"type\":{\"name\":\"Fleur\"},\"updatedAt\":\"Sat, 04 May 2019 23:45:59 +0000\"}}}");
            else
                query(graphQL.GetPlantData(externalData.plants["Fleur"]["Pétunia"].plantID));
        }
    }

    //Link To REACT
    public void ExportScene()
    {
        SerializationController.instance.Serialize();
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
    }

    public void DispatchQueryResult(string json)
    {
        var jsonObject = JSONObject.Parse(json);
        if (jsonObject["errors"] != null)
        {
            ErrorHandler.instance.ErrorMessage(jsonObject["errors"].Value);
            return;
        }
        var keys = jsonObject["data"].Keys;
        keys.MoveNext();
        Action<string> tmp = callbacks[keys.Current.Value];
        if (tmp != null)
            tmp.Invoke(json);
    }

    //Link to UI
    public void GetPlantsType()
    {
        if (Application.isEditor)
            DispatchQueryResult("{\"data\":{\"getAllPlants\":{\"Fleur\":[{\"node\":{\"name\":\"Abricotier\"}},{\"node\":{\"name\":\"Campanule\"}},{\"node\":{\"name\":\"Capucine\"}},{\"node\":{\"name\":\"Coquelicot\"}},{\"node\":{\"name\":\"Crocus\"}},{\"node\":{\"name\":\"Edelweiss\"}},{\"node\":{\"name\":\"Gardénia\"}},{\"node\":{\"name\":\"Jacinthe\"}},{\"node\":{\"name\":\"Lys\"}},{\"node\":{\"name\":\"Narcisse\"}},{\"node\":{\"name\":\"Œillet\"}},{\"node\":{\"name\":\"Oeillet d'Inde\"}},{\"node\":{\"name\":\"Orchidées\"}},{\"node\":{\"name\":\"Pensée\"}},{\"node\":{\"name\":\"Pétunia\"}},{\"node\":{\"name\":\"Phalaenopsis\"}},{\"node\":{\"name\":\"Pivoine\"}}]}}}");
        else
            query(graphQL.GetPlantsOfType("Fleur", externalData.plantsTypes["Fleur"]));
    }

    public void GetPlantsData()
    {
        if (Application.isEditor)
            DispatchQueryResult("{\"data\":{\"getPlant\":{\"name\":\"Pétunia\",\"blossomingEnd\":[4],\"blossomingStart\":[2],\"color\":[\"Vert\",\"Rose\"],\"createdAt\":\"Sat, 04 May 2019 23:45:58 +0000\",\"groundTypes\":[{\"id\":\"e060619a-7977-4280-ba21-f5673ebb6817\"},{\"id\":\"e09f04bf-57a0-45cc-8e89-00f3657bd287\"},{\"id\":\"f6ddd7f8-329e-4809-a869-40281392823b\"}],\"heightLow\":10000,\"heightHigh\":null,\"id\":\"6bcc1cc7-32dd-4e38-b516-7b70d341707f\",\"periodicities\":[{\"name\":\"Vivace\"}],\"phRangeLow\":7,\"phRangeHigh\":7,\"photo\":\"https://s3.greefine.ovh/dev/8cd324ca744800af98885ec7378bf5ad390fe3ea/e57ce8f2-a9b8-4de8-94a7-d7efea5ccffd.jpg\",\"rusticity\":5,\"shapes\":[{\"name\":\"Arrondi\"}],\"sunNeed\":8,\"thumbnail\":\"https://s3.greefine.ovh/dev/8cd324ca744800af98885ec7378bf5ad390fe3ea/thumbnail_e57ce8f2-a9b8-4de8-94a7-d7efea5ccffd.jpg\",\"type\":{\"name\":\"Fleur\"},\"updatedAt\":\"Sat, 04 May 2019 23:45:59 +0000\"}}}");
        else
            query(graphQL.GetPlantData(externalData.plants["Fleur"]["Pétunia"].plantID));
    }
}
