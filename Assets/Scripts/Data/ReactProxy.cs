using SimpleJSON;
using System;
using System.Collections;
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
    public Dictionary<string, Action<PlantData, GameObject>> plantCallbacks = new Dictionary<string, Action<PlantData, GameObject>>();
    public ModelSO modelList;
    public GameObject fallbackModel;
    public bool ready = false;

    [DllImport("__Internal")]
    private static extern void save(string json);
    [DllImport("__Internal")]
    private static extern void unsavedWork(bool status);
    [DllImport("__Internal")]
    private static extern void query(string payload);

    private bool saveLock = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            graphQL = new GraphQL();
            externalData = new ExternalData(callbacks);
            if (Application.isEditor)
                DispatchQueryResult("{\"data\":{\"getTypes\":[{\"name\":\"Arbre\",\"id\":\"ae68c6e8-9d0c-4fdc-9e72-8e1896e3dfa8\",\"__typename\":\"Type\"},{\"name\":\"Fleur\",\"id\":\"76f0f28d-42da-45ad-91db-e227b12fb71e\",\"__typename\":\"Type\"},{\"name\":\"Arbuste\",\"id\":\"763b5496-02a3-4cdb-9241-9f043a2d4db5\",\"__typename\":\"Type\"},{\"name\":\"Legume\",\"id\":\"47c12932-69ef-4925-9fd3-b263fc47d69a\",\"__typename\":\"Type\"},{\"name\":\"typeKEYVAL3\",\"id\":\"264ac357-5d66-4d3e-90aa-1c89a9f78b1e\",\"__typename\":\"Type\"}]}}");
            else
                SendQuery(graphQL.GetPlantsTypes());
            if (Application.isEditor)
                DispatchQueryResult("{\"data\":{\"getGroundTypes\":[{\"name\":\"Calcaire\",\"id\":\"4cd8dbde-2a69-4117-9ab5-a6536a436b56\"},{\"name\":\"Sableux\",\"id\":\"ba35baed-dea6-4bfb-a46b-85eaffb9d018\"},{\"name\":\"Caillouteux\",\"id\":\"a0f11e91-bbbd-4487-a77c-8f9cc2f04866\"},{\"name\":\"Argileux\",\"id\":\"51905da7-cd16-4db3-8dcc-da70a8ef77d7\"},{\"name\":\"Humus\",\"id\":\"c284f849-85e8-4b20-8bb9-dc28888d62d9\"},{\"name\":\"Bruyère\",\"id\":\"58270831-9039-43ba-82ec-393244bea76a\"},{\"name\":\"Calliouteux\",\"id\":\"91569814-4ab6-49ad-b8b5-9a594f05b73d\"},{\"name\":\"Terreau\",\"id\":\"dcce412d-13e5-413e-b3b9-90ce95538c81\"},{\"name\":\"Calaire\",\"id\":\"edbfd600-e1cd-4104-9790-e378a47d66eb\"},{\"name\":\"Hulus\",\"id\":\"e2ff6365-58c6-4d03-b15a-b6b55cf028db\"}]}}");
            else
                SendQuery(graphQL.GetGroundTypes());
        }
        else
            Destroy(this);
    }

    private void Start()
    {
        //Fake InitScene
        if (Application.isEditor)
        {
            SerializationController.instance.GetComponent<GardenData>().SetGardenName("Offline Garden");
            LocalisationController.instance.Init("FR");//TODO USERPREF
        }
    }

    //Link To REACT
    public void ExportScene()
    {
        if (Application.isEditor)
        {
            Debug.Log(SerializationController.instance.Serialize());
            OnSaveResult("{\"success\":true}");
        }
        else
        {
            string tmp = SerializationController.instance.Serialize();

            if (tmp != null)
                save(tmp);
        }
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
        {
            //SerializationController.instance.GetComponent<GardenData>().SetGardenName(JSONObject.Parse(json)["name"]);
            LocalisationController.instance.Init("FR");//TODO USERPREF
            StartCoroutine(LoadScene(json));
        }
    }

    public IEnumerator LoadScene(string json)
    {
        while (!ready)
            yield return new WaitForSeconds(.1f);
        SerializationController.instance.DeSerialize(json);
        yield return null;
    }

    public void DispatchQueryResult(string json)
    {
        var jsonObject = JSONObject.Parse(json);
        if (jsonObject["errors"] != null)
        {
            MessageHandler.instance.ErrorMessage("loading_error");
            return;
        }
        var keys = jsonObject["data"].Keys;
        keys.MoveNext();
        callbacks[keys.Current.Value].Invoke(json);
    }

    public void OnSaveResult(string json)
    {
        if (json != null && json != "")
        {
            var jsonObject = JSONObject.Parse(json);
            if (jsonObject != null && jsonObject["success"] != null && jsonObject["success"].AsBool)
                saveLock = false;
        }
        if (!saveLock)
        {
            PlayerController.instance.actionHandler.OnSaveSucessfull();
            SerializationController.instance.OnSaveSucessfull();
            MessageHandler.instance.SuccesMessage("save_sucessfull");
        }
        else
            MessageHandler.instance.ErrorMessage("failed_save");

        if (saveLock)
        {
            saveLock = false;
            PlayerController.instance.actionHandler.OnSaveSucessfull();
            SerializationController.instance.OnSaveSucessfull();
            MessageHandler.instance.SuccesMessage("save_sucessfull");
        }
    }

    //Link to UI
    public string[] GetPlantsType(string plantType)
    {
        if (!externalData.plants.ContainsKey(plantType))
            return null;
        if (externalData.plants[plantType].Keys.Count == 0)
        {
            if (Application.isEditor)
                DispatchQueryResult("{\"data\":{\"getAllPlants\":{\"Fleur\":[{\"node\":{\"name\":\"Anémone\",\"id\":\"3abb8289-3847-4c2b-a1bc-b696ee581c87\"}},{\"node\":{\"name\":\"Campanule\",\"id\":\"c00730f7-908d-4509-a3a8-230d39522248\"}},{\"node\":{\"name\":\"Capucine\",\"id\":\"850f9e79-ee8b-4f02-b605-a20bcdf7a58d\"}},{\"node\":{\"name\":\"Coquelicot\",\"id\":\"eefd68a1-97d4-4965-867e-755d1b609de6\"}},{\"node\":{\"name\":\"Crocus\",\"id\":\"0a7eb1e3-2f2e-4287-8411-80ab463d202c\"}},{\"node\":{\"name\":\"Edelweiss\",\"id\":\"909c93d6-532e-4818-907a-129c328d21cc\"}},{\"node\":{\"name\":\"Gardénia\",\"id\":\"6a1b69e1-bdb9-47c6-9cbd-fd62b04bd59c\"}},{\"node\":{\"name\":\"Jacinthe\",\"id\":\"67864e0e-46e1-4fd1-a8ea-62f98d8fd5ba\"}},{\"node\":{\"name\":\"Lys\",\"id\":\"be705d09-ff94-40b2-9bb4-299505ad9e94\"}},{\"node\":{\"name\":\"Narcisse\",\"id\":\"1ae7f5fd-d5fa-42f0-b137-05cc7267bdee\"}},{\"node\":{\"name\":\"Œillet\",\"id\":\"1d56de6c-0147-4a31-91e8-4ed6a97e1d75\"}},{\"node\":{\"name\":\"Oeillet d'Inde\",\"id\":\"b32e9595-80ef-4162-8510-a2c7b49eb569\"}},{\"node\":{\"name\":\"Orchidées\",\"id\":\"3b97b479-2316-40a3-a92f-342d18d08ffe\"}},{\"node\":{\"name\":\"Pensée\",\"id\":\"05dd5581-c641-4464-800a-3c4ec993e9f1\"}},{\"node\":{\"name\":\"Pétunia\",\"id\":\"3df07e68-1af1-40ef-bf17-4b412e80574b\"}},{\"node\":{\"name\":\"Phalaenopsis\",\"id\":\"25b08798-b9e4-4078-a1b9-0abe48f5b74d\"}},{\"node\":{\"name\":\"Pivoine\",\"id\":\"ff2e5c63-56d3-431f-b8c9-033f2909c1f0\"}},{\"node\":{\"name\":\"Primevère\",\"id\":\"5dd7a14a-a151-4585-9996-0c8491160ddf\"}},{\"node\":{\"name\":\"Renoncule\",\"id\":\"7ace3c1f-4f94-4e37-b44c-cfd0fd3ed350\"}},{\"node\":{\"name\":\"Souci\",\"id\":\"517140c6-f734-43f7-9bf1-b3044b52c109\"}},{\"node\":{\"name\":\"Tulipe\",\"id\":\"d234c857-1353-4e19-99eb-841f5f3df363\"}},{\"node\":{\"name\":\"Violette\",\"id\":\"f9a52114-b03a-492a-9fc7-0bcfc26ca867\"}},{\"node\":{\"name\":\"Zinnia\",\"id\":\"fd3df876-29e1-48ee-8c5d-a1dcf3638cc7\"}}]}}}");
            else if (externalData.plants[plantType].Keys.Count == 0)
                SendQuery(graphQL.GetPlantsOfType(plantType, externalData.plantsTypes[plantType]));
            return null;
        }

        return externalData.plants[plantType].Values.Select(x => x.name).ToArray();
    }

    public PlantData GetPlantsData(string plantType, string plantName)
    {
        if (!externalData.plants.ContainsKey(plantType)
            || externalData.plants[plantType][plantName].status == PlantData.DataStatus.Requested)
        {
            return null;
        }
        else if (!externalData.plants[plantType].ContainsKey(plantName)
            || externalData.plants[plantType][plantName].status == PlantData.DataStatus.None)
        {
            if (Application.isEditor)
                DispatchQueryResult("{\"data\":{\"getPlant\":{\"name\":\"Pétunia\",\"type\":{\"name\":\"Fleur\",\"id\":\"8bae24ec-6ac6-4059-9a0e-0cdcb2602a7a\"},\"id\":\"3df07e68-1af1-40ef-bf17-4b412e80574b\",\"colors\":[{\"name\":\"Rose\"},{\"name\":\"Blanche\"},{\"name\":\"Orange\"},{\"name\":\"Rouge\"},{\"name\":\"Jaune\"},{\"name\":\"Violet\"},{\"name\":\"Bleu\"}],\"phRangeLow\":0,\"phRangeHigh\":7,\"rusticity\":5,\"sunNeed\":7,\"waterNeed\":9,\"description\":\"Le pétunia est une fleur facile d’entretien. Sa floraison longue et abondante, aux couleurs variées et éclatantes, s'épanouit du printemps jusqu’aux premières gelées. En jardinière ou en suspension, il est la star des balcons et rebords de fenêtre.\",\"model\":2,\"thumbnail\":\"https://s3.gardenly.app/dev/6ea11cc99fe9a6f7c5a7cdeebf80d5393da23853/thumbnail_f98e393a-8f09-4cd4-9b93-9da72873ccf6.jpg\"}}}");
            else
                SendQuery(graphQL.GetPlantData(externalData.plants[plantType][plantName].plantID));
            externalData.plants[plantType][plantName].status = PlantData.DataStatus.Requested;
            return null;
        }
        return externalData.plants[plantType][plantName];
    }

    public void LoadPlantDataFromSave(Action<PlantData, GameObject> callback, string plantID, string plantName, string plantType)
    {
        if (!ready
            || !externalData.plants.ContainsKey(plantType)
            || !externalData.plants[plantType].ContainsKey(plantName))
        {
            plantCallbacks.Add(plantID, callback);
            externalData.callbackLoadData.Add(plantID, LoadPlantDataCallback);
            if (!externalData.plants.ContainsKey(plantType)
                || !externalData.plants[plantType].ContainsKey(plantName))
                SendQuery(graphQL.GetPlantData(plantID));
        }
        else
        {
            PlantData tmp = externalData.plants[plantType][plantName];
            callback.Invoke(tmp, null);
        }
    }

    public void LoadPlantDataCallback(PlantData plantData)
    {
        if (plantData.model >= 0 && plantData.model < modelList.Models.Count)
            plantCallbacks[plantData.plantID].Invoke(plantData, modelList.Models[plantData.model]);
        else
            plantCallbacks[plantData.plantID].Invoke(plantData, fallbackModel);
    }
}
