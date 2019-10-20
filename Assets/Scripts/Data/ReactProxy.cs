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
                DispatchQueryResult("{\"data\":{\"getTypes\":[{\"name\":\"Arbre\",\"id\":\"40219c47-0e6a-494f-82b5-a67fc984af23\"},{\"name\":\"Arbuste\",\"id\":\"b2486c82-9452-4407-8a6b-fea79e94f9f5\"},{\"name\":\"Fleur\",\"id\":\"8bae24ec-6ac6-4059-9a0e-0cdcb2602a7a\"},{\"name\":\"Legume\",\"id\":\"98267617-3720-46f3-9c6f-8970061ad7e8\"}]}}");
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

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    InitScene("{\"id\":\"f5af5fee-92df-4868-8b7a-766a82702987\",\"name\":\"Paris 2\",\"country\":null,\"slug\":\"paris-2\",\"tiles\":[{\"id\":\"72ff9455-d0e3-410e-897a-983d2a38b063\",\"key\":1569574400,\"name\":\"Parterre 1\",\"data\":\"{\\\"points\\\":[{\\\"x\\\":41.0,\\\"y\\\":-61.0},{\\\"x\\\":46.0,\\\"y\\\":-61.0},{\\\"x\\\":46.0,\\\"y\\\":-55.0},{\\\"x\\\":41.0,\\\"y\\\":-55.0}]}\",\"groundType\":{\"id\":\"0162a5af-0a36-4deb-8365-059e1d70b200\",\"name\":\"Calcaire\",\"__typename\":\"GroundType\"},\"__typename\":\"Tile\"},{\"id\":\"cd82d698-af6c-4c16-8dbe-b4ae31c34253\",\"key\":1569574400,\"name\":\"Parterre 2\",\"data\":\"{\\\"points\\\":[{\\\"x\\\":54.0,\\\"y\\\":-61.0},{\\\"x\\\":59.0,\\\"y\\\":-61.0},{\\\"x\\\":59.0,\\\"y\\\":-55.0},{\\\"x\\\":54.0,\\\"y\\\":-55.0}]}\",\"groundType\":{\"id\":\"0162a5af-0a36-4deb-8365-059e1d70b200\",\"name\":\"Calcaire\",\"__typename\":\"GroundType\"},\"__typename\":\"Tile\"}],\"staticElements\":[{\"id\":\"73f33a98-ac4d-4304-be32-78b325126806\",\"key\":1569574312,\"data\":\"{\\\"type\\\":\\\"Wall\\\",\\\"start\\\":{\\\"x\\\":47.0,\\\"y\\\":0.0,\\\"z\\\":-47.0},\\\"end\\\":{\\\"x\\\":40.0,\\\"y\\\":0.0,\\\"z\\\":-47.0},\\\"rotation\\\":{\\\"x\\\":0.0,\\\"y\\\":0.0,\\\"z\\\":0.0,\\\"w\\\":1.0}}\",\"__typename\":\"StaticElement\"},{\"id\":\"5b65a8e1-55ab-4cc4-b24b-bfbc6bb4b06a\",\"key\":1569574322,\"data\":\"{\\\"type\\\":\\\"Wall\\\",\\\"start\\\":{\\\"x\\\":40.0,\\\"y\\\":0.0,\\\"z\\\":-47.0},\\\"end\\\":{\\\"x\\\":40.0,\\\"y\\\":0.0,\\\"z\\\":-62.0},\\\"rotation\\\":{\\\"x\\\":0.0,\\\"y\\\":0.7071068286895752,\\\"z\\\":0.0,\\\"w\\\":-0.7071068286895752}}\",\"__typename\":\"StaticElement\"},{\"id\":\"4bf1fd77-74a2-4c70-90a4-753dadbd1e1e\",\"key\":1569574328,\"data\":\"{\\\"type\\\":\\\"Wall\\\",\\\"start\\\":{\\\"x\\\":40.0,\\\"y\\\":0.0,\\\"z\\\":-62.0},\\\"end\\\":{\\\"x\\\":48.0,\\\"y\\\":0.0,\\\"z\\\":-62.0},\\\"rotation\\\":{\\\"x\\\":0.0,\\\"y\\\":1.0,\\\"z\\\":0.0,\\\"w\\\":0.0}}\",\"__typename\":\"StaticElement\"},{\"id\":\"6e9b833a-d5a1-40d8-ad42-faa7c07a9e3f\",\"key\":1569574357,\"data\":\"{\\\"type\\\":\\\"Wall\\\",\\\"start\\\":{\\\"x\\\":52.0,\\\"y\\\":0.0,\\\"z\\\":-62.0},\\\"end\\\":{\\\"x\\\":60.0,\\\"y\\\":0.0,\\\"z\\\":-62.0},\\\"rotation\\\":{\\\"x\\\":0.0,\\\"y\\\":1.0,\\\"z\\\":0.0,\\\"w\\\":0.0}}\",\"__typename\":\"StaticElement\"},{\"id\":\"cf319f94-ff06-43bb-b143-a0428b5e07cd\",\"key\":1569574368,\"data\":\"{\\\"type\\\":\\\"Wall\\\",\\\"start\\\":{\\\"x\\\":60.0,\\\"y\\\":0.0,\\\"z\\\":-62.0},\\\"end\\\":{\\\"x\\\":60.0,\\\"y\\\":0.0,\\\"z\\\":-47.0},\\\"rotation\\\":{\\\"x\\\":0.0,\\\"y\\\":0.7071068286895752,\\\"z\\\":0.0,\\\"w\\\":0.7071068286895752}}\",\"__typename\":\"StaticElement\"},{\"id\":\"bb7e6767-bf4f-4037-a67c-451c1af2b057\",\"key\":1569574373,\"data\":\"{\\\"type\\\":\\\"Wall\\\",\\\"start\\\":{\\\"x\\\":60.0,\\\"y\\\":0.0,\\\"z\\\":-47.0},\\\"end\\\":{\\\"x\\\":52.811214447021487,\\\"y\\\":-9.5367431640625e-7,\\\"z\\\":-47.02238845825195},\\\"rotation\\\":{\\\"x\\\":-1.0328804478376696e-10,\\\"y\\\":-0.0015571415424346924,\\\"z\\\":6.633030125158257e-8,\\\"w\\\":0.9999988079071045}}\",\"__typename\":\"StaticElement\"}],\"plants\":[{\"id\":\"5e204e93-c62c-458f-87eb-db85b8f9459f\",\"data\":\"{\\\"position\\\":{\\\"x\\\":56.424747467041019,\\\"y\\\":0.0,\\\"z\\\":-55.953712463378909}}\",\"key\":1569574545,\"age\":null,\"plant\":{\"id\":\"b5b84d24-7c13-466e-8b8d-61ba6bd2c364\",\"name\":\"Bougainvillier\",\"__typename\":\"Plant\"},\"sunExposition\":0,\"__typename\":\"PlantTile\"},{\"id\":\"2516e25f-f34d-45ce-9beb-5b468c970d71\",\"data\":\"{\\\"position\\\":{\\\"x\\\":56.35068130493164,\\\"y\\\":0.0,\\\"z\\\":-59.84307098388672}}\",\"key\":1569574554,\"age\":null,\"plant\":{\"id\":\"fb59f818-8a68-4a6b-9cac-19c0be259c65\",\"name\":\"Lys\",\"__typename\":\"Plant\"},\"sunExposition\":0,\"__typename\":\"PlantTile\"},{\"id\":\"dbfc115e-26a9-4918-b8b6-783d383f0898\",\"data\":\"{\\\"position\\\":{\\\"x\\\":56.3575325012207,\\\"y\\\":0.0,\\\"z\\\":-58.44941711425781}}\",\"key\":1569574564,\"age\":null,\"plant\":{\"id\":\"1bd6ab04-5f34-4e2f-a330-513a3721187d\",\"name\":\"Coquelicot\",\"__typename\":\"Plant\"},\"sunExposition\":0,\"__typename\":\"PlantTile\"},{\"id\":\"b19dc2f4-1ac5-45e4-988b-f96aa4ee5274\",\"data\":\"{\\\"position\\\":{\\\"x\\\":43.49794387817383,\\\"y\\\":0.0,\\\"z\\\":-55.73651123046875}}\",\"key\":1569574571,\"age\":null,\"plant\":{\"id\":\"f9d7aa36-48b3-4b9c-b225-8030c441273b\",\"name\":\"Anémone\",\"__typename\":\"Plant\"},\"sunExposition\":0,\"__typename\":\"PlantTile\"},{\"id\":\"c5ebeb42-748b-4384-9bf6-ab1eb4cdcef4\",\"data\":\"{\\\"position\\\":{\\\"x\\\":43.52072525024414,\\\"y\\\":0.0,\\\"z\\\":-59.97710418701172}}\",\"key\":1569574586,\"age\":null,\"plant\":{\"id\":\"e2af9d94-beb0-4a59-b8f9-f1113f49cf16\",\"name\":\"Carotte\",\"__typename\":\"Plant\"},\"sunExposition\":0,\"__typename\":\"PlantTile\"},{\"id\":\"ed85ba46-abb2-4c69-b163-5aa3f43e1f52\",\"data\":\"{\\\"position\\\":{\\\"x\\\":43.245025634765628,\\\"y\\\":0.0,\\\"z\\\":-58.084354400634769}}\",\"key\":1569574589,\"age\":null,\"plant\":{\"id\":\"e66c677a-724a-4788-88b4-46b0178754db\",\"name\":\"Pomme de terre\",\"__typename\":\"Plant\"},\"sunExposition\":0,\"__typename\":\"PlantTile\"}],\"data\":\"\\\"{}\\\"\",\"__typename\":\"Garden\"}");
        //}
    }

    //Link To REACT
    public void ExportScene()
    {
        if (saveLock)
            return;
        saveLock = true;

        if (Application.isEditor)
        {
            Debug.Log(SerializationController.instance.Serialize());
            OnSaveResult("{\"success\":true}");
        }
        else
        {
            string tmp = SerializationController.instance.Serialize();
            Debug.Log("Save <" + tmp + ">");
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
            SerializationController.instance.DeSerialize(json);
        }
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
    private IEnumerator TmpFix()
    {
        yield return new WaitForSeconds(2);
        ExportScene();

    }
    public void OnSaveResult(string json)
    {
        Debug.Log("OnSaveResult: <" + json + ">");

        //if (json != null && json != "")
        //{
        //    var jsonObject = JSONObject.Parse(json);
        //    if (jsonObject != null && jsonObject["success"] != null && jsonObject["success"].AsBool)
        //    {
        //        saveLock = false;
        //        Debug.Log("UNLOCK");
        //    }
        //}
        PlayerController.instance.actionHandler.OnSaveSucessfull();
        if (SerializationController.instance.OnSaveSucessfull())
        {
            MessageHandler.instance.SuccesMessage("save_sucessfull");
            saveLock = false;
        }
        else
            StartCoroutine(TmpFix());//ExportScene();
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
            || !externalData.plants[plantType].ContainsKey(plantName)
            || externalData.plants[plantType][plantName].status == PlantData.DataStatus.Requested)
            return null;
        if (externalData.plants[plantType][plantName].status == PlantData.DataStatus.None)
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

    public void LoadPlantDataFromId(string plantID, Action<PlantData> callback)
    {
        SendQuery(graphQL.GetPlantData(plantID));
    }
}
