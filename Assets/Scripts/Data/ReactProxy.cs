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

    //Link To REACT
    public void ExportScene()
    {
        if (Application.isEditor)
            Debug.Log(SerializationController.instance.Serialize());
        else
            save(SerializationController.instance.Serialize());
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad5))
            InitScene("{\"id\":\"6ee8a7c1-342a-49e1-bd2e-908cb0a61462\",\"name\":\"Mon troisième jardin\",\"country\":null,\"slug\":\"mon-troisieme-jardin\",\"tiles\":[{\"id\":\"9f0d43cd-79ea-4e37-9550-c5ebe01e0029\",\"key\":1560899818,\"name\":\"Parterre 1\",\"data\":\"{\\\"points\\\":[{\\\"x\\\":57.75991439819336,\\\"y\\\":-55.8231086730957},{\\\"x\\\":66.21114349365235,\\\"y\\\":-44.016265869140628},{\\\"x\\\":64.1568832397461,\\\"y\\\":-60.33591842651367},{\\\"x\\\":54.20306396484375,\\\"y\\\":-63.12013626098633},{\\\"x\\\":51.52617645263672,\\\"y\\\":-62.991729736328128},{\\\"x\\\":49.015380859375,\\\"y\\\":-61.69110870361328},{\\\"x\\\":47.57565689086914,\\\"y\\\":-60.30885314941406},{\\\"x\\\":51.48556900024414,\\\"y\\\":-55.12522506713867},{\\\"x\\\":52.94807815551758,\\\"y\\\":-55.555667877197269},{\\\"x\\\":54.708621978759769,\\\"y\\\":-56.159976959228519},{\\\"x\\\":56.008365631103519,\\\"y\\\":-56.30715560913086}]}\",\"groundType\":{\"id\":\"e060619a-7977-4280-ba21-f5673ebb6817\",\"name\":\"Calcaire\",\"__typename\":\"GroundType\"},\"__typename\":\"Tile\"},{\"id\":\"7a58fb17-26f4-4c34-afdf-f23cae5a5db9\",\"key\":1560899818,\"name\":\"Parterre 1\",\"data\":\"{\\\"points\\\":[{\\\"x\\\":57.75991439819336,\\\"y\\\":-55.8231086730957},{\\\"x\\\":66.21114349365235,\\\"y\\\":-44.016265869140628},{\\\"x\\\":64.1568832397461,\\\"y\\\":-60.33591842651367},{\\\"x\\\":54.20306396484375,\\\"y\\\":-63.12013626098633},{\\\"x\\\":51.52617645263672,\\\"y\\\":-62.991729736328128},{\\\"x\\\":49.015380859375,\\\"y\\\":-61.69110870361328},{\\\"x\\\":47.57565689086914,\\\"y\\\":-60.30885314941406},{\\\"x\\\":51.48556900024414,\\\"y\\\":-55.12522506713867},{\\\"x\\\":52.94807815551758,\\\"y\\\":-55.555667877197269},{\\\"x\\\":54.708621978759769,\\\"y\\\":-56.159976959228519},{\\\"x\\\":56.008365631103519,\\\"y\\\":-56.30715560913086}]}\",\"groundType\":{\"id\":\"e060619a-7977-4280-ba21-f5673ebb6817\",\"name\":\"Calcaire\",\"__typename\":\"GroundType\"},\"__typename\":\"Tile\"}],\"staticElements\":[{\"id\":\"b103fc68-c2a1-4bd1-b69e-c3ad64c742f5\",\"key\":1560899827,\"data\":\"{\\\"type\\\":\\\"Wall\\\",\\\"start\\\":{\\\"x\\\":57.995704650878909,\\\"y\\\":-9.5367431640625e-7,\\\"z\\\":-55.5169563293457},\\\"end\\\":{\\\"x\\\":66.05252075195313,\\\"y\\\":-9.5367431640625e-7,\\\"z\\\":-44.23919677734375}}\",\"__typename\":\"StaticElement\"},{\"id\":\"3392da79-ff56-41a0-a5ae-359a1af9725e\",\"key\":1560899839,\"data\":\"{\\\"type\\\":\\\"Wall\\\",\\\"start\\\":{\\\"x\\\":64.10964965820313,\\\"y\\\":0.0,\\\"z\\\":-60.33591842651367},\\\"end\\\":{\\\"x\\\":54.242088317871097,\\\"y\\\":0.0,\\\"z\\\":-63.141475677490237}}\",\"__typename\":\"StaticElement\"},{\"id\":\"d9644684-33a5-4805-8f30-71479473fa6b\",\"key\":1560899842,\"data\":\"{\\\"type\\\":\\\"Table\\\",\\\"position\\\":{\\\"x\\\":63.0,\\\"y\\\":0.0,\\\"z\\\":-57.0},\\\"rotation\\\":{\\\"x\\\":-0.7071068286895752,\\\"y\\\":0.0,\\\"z\\\":0.0,\\\"w\\\":0.7071068286895752}}\",\"__typename\":\"StaticElement\"},{\"id\":\"afb4f9d5-1031-4269-866b-ed5a4f6c74eb\",\"key\":1560899845,\"data\":\"{\\\"type\\\":\\\"Chair\\\",\\\"position\\\":{\\\"x\\\":63.11836624145508,\\\"y\\\":0.0,\\\"z\\\":-56.159976959228519},\\\"rotation\\\":{\\\"x\\\":-0.7071068286895752,\\\"y\\\":0.0,\\\"z\\\":0.0,\\\"w\\\":0.7071068286895752}}\",\"__typename\":\"StaticElement\"},{\"id\":\"1301a9ff-6ccc-4e34-987d-36af6b44f1ea\",\"key\":1560899847,\"data\":\"{\\\"type\\\":\\\"Chair\\\",\\\"position\\\":{\\\"x\\\":61.765811920166019,\\\"y\\\":0.0,\\\"z\\\":-57.0559196472168},\\\"rotation\\\":{\\\"x\\\":-0.7071068286895752,\\\"y\\\":0.0,\\\"z\\\":0.0,\\\"w\\\":0.7071068286895752}}\",\"__typename\":\"StaticElement\"}],\"plants\":[],\"data\":\"\\\"{}\\\"\",\"__typename\":\"Garden\"}");
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
            {
                if (plantName == "Pétunia")
                    DispatchQueryResult("{\"data\":{\"getPlant\":{\"name\":\"Pétunia\",\"type\":{\"name\":\"Fleur\",\"id\":\"8bae24ec-6ac6-4059-9a0e-0cdcb2602a7a\"},\"id\":\"3df07e68-1af1-40ef-bf17-4b412e80574b\",\"colors\":[{\"name\":\"Rose\"},{\"name\":\"Blanche\"},{\"name\":\"Orange\"},{\"name\":\"Rouge\"},{\"name\":\"Jaune\"},{\"name\":\"Violet\"},{\"name\":\"Bleu\"}],\"phRangeLow\":0,\"phRangeHigh\":7,\"rusticity\":5,\"sunNeed\":7,\"waterNeed\":9,\"description\":\"Le pétunia est une fleur facile d’entretien. Sa floraison longue et abondante, aux couleurs variées et éclatantes, s'épanouit du printemps jusqu’aux premières gelées. En jardinière ou en suspension, il est la star des balcons et rebords de fenêtre.\",\"model\":2,\"thumbnail\":\"https://s3.gardenly.app/dev/6ea11cc99fe9a6f7c5a7cdeebf80d5393da23853/thumbnail_f98e393a-8f09-4cd4-9b93-9da72873ccf6.jpg\"}}}");
                else
                    return null;
            }
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
