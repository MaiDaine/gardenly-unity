using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExternalData
{
    public KeyValuePair<string, string>[] groundTypes;
    public List<Action> callbackGround = new List<Action>();
    public Dictionary<string, string> plantsTypes = new Dictionary<string, string>();
    public Dictionary<string, Dictionary<string, PlantData>> plants = new Dictionary<string, Dictionary<string, PlantData>>();
    public Dictionary<string, Action<PlantData>> callbackLoadData = new Dictionary<string, Action<PlantData>>();
    public Dictionary<string, Action<Texture2D>> callbackFinishDownloadImage = new Dictionary<string, Action<Texture2D>>();

    public ExternalData(Dictionary<string, Action<string>> callbacks)
    {
        callbacks["getGroundTypes"] = SetGroundTypes;
        callbacks["getTypes"] = SetPlantsTypesNames;
        callbacks["getAllPlants"] = SetPlantOfType;
        callbacks["getPlant"] = SetPlantData;
    }

    private void SetGroundTypes(string json)
    {
        var jsonObject = JSONObject.Parse(json);

        int count = jsonObject["data"]["getGroundTypes"].Count;
        groundTypes = new KeyValuePair<string, string>[count];
        for (int i = 0; i < count; i++)
        {
            var tmp = jsonObject["data"]["getGroundTypes"][i];
            groundTypes[i] = new KeyValuePair<string, string>(tmp["name"], tmp["id"]);
        }
        foreach (Action action in callbackGround)
            action.Invoke();
        callbackGround.Clear();
        callbackGround = null;
    }

    private void SetPlantsTypesNames(string json)
    {
        var jsonObject = JSONObject.Parse(json);

        int count = jsonObject["data"]["getTypes"].Count;
        for (int i = 0; i < count; i++)
        {
            var tmp = jsonObject["data"]["getTypes"][i];
            plantsTypes[tmp["name"]] = tmp["id"];
            plants[tmp["name"]] = new Dictionary<string, PlantData>();

            if (Application.isEditor)
                ReactProxy.instance.GetPlantsType("Fleur");
            else
                ReactProxy.instance.GetPlantsType(tmp["name"]);
        }
    }

    private void SetPlantOfType(string json)
    {
        var jsonObject = JSONObject.Parse(json);
        var tmp = jsonObject["data"]["getAllPlants"].Keys;
        tmp.MoveNext();
        string plantType = tmp.Current.Value;
        int count = jsonObject["data"]["getAllPlants"][plantType].Count;
        for (int i = 0; i < count; i++)
        {
            var tmpPlant = jsonObject["data"]["getAllPlants"][plantType][i]["node"];
            string name = tmpPlant["name"];
            plants[plantType][name] = new PlantData(name);
            plants[plantType][name].typeName = plantType;
            plants[plantType][name].plantID = tmpPlant["id"];
        }
        ReactProxy.instance.ready = true;
    }

    private void SetPlantData(string json)
    {
        var jsonObject = JSONObject.Parse(json);
        var tmp = jsonObject["data"]["getPlant"];
        PlantData plantData = plants[tmp["type"]["name"]][tmp["name"]];
        plantData.typeName = tmp["type"]["name"];
        plantData.plantID = tmp["id"];
        plantData.plantColor = new string[tmp["colors"].AsArray.Count];
        for (int i = 0; i < tmp["colors"].AsArray.Count; i++)
            plantData.plantColor[i] = tmp["colors"].AsArray[i]["name"];
        plantData.phRangeLow = tmp["phRangeLow"];
        plantData.phRangeHigh = tmp["phRangeHigh"];
        plantData.rusticity = tmp["rusticity"];
        plantData.sunNeed = tmp["sunNeed"];
        plantData.waterNeed = tmp["waterNeed"];
        plantData.description = tmp["description"];
        plantData.model = tmp["model"];
        plantData.imgUrl = tmp["thumbnail"];
        plantData.status = PlantData.DataStatus.Received;
        if (plantData.plantID != null)
        {
            if (callbackLoadData.ContainsKey(plantData.name))
                callbackLoadData[plantData.name].Invoke(plantData);
            if (ReactProxy.instance.plantCallbacks.ContainsKey(plantData.plantID))
                ReactProxy.instance.LoadPlantDataCallback(plantData);
        }
    }

    public IEnumerator GetTexture(PlantData plantData, string imageUrl)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
            Debug.Log(www.error);
        else
            plantData.image = ((DownloadHandlerTexture)www.downloadHandler).texture;
        if (callbackFinishDownloadImage.ContainsKey(plantData.name))
            callbackFinishDownloadImage[plantData.name].Invoke(plantData.image);
    }
}