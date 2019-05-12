using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExternalData : MonoBehaviour
{
    public Dictionary<string, string> plantsTypes = new Dictionary<string, string>();
    public Dictionary<string, Dictionary<string, PlantData>> plants = new Dictionary<string, Dictionary<string, PlantData>>();
    public Action<string> callbackFinishDownloadImage;

    public void Init(Dictionary<string, Action<string>> callbacks)
    {
        callbacks["getTypes"] = SetPlantsTypes;
        callbacks["getAllPlants"] = SetPlantOfType;
        callbacks["getPlant"] = SetPlantData;
    }

    public void SetPlantsTypes(string json)
    {
        var jsonObject = JSONObject.Parse(json);

        int count = jsonObject["data"]["getTypes"].Count;
        for (int i = 0; i < count; i++)
        {
            var tmp = jsonObject["data"]["getTypes"][i];
            plantsTypes[tmp["name"]] = tmp["id"];
            plants[tmp["name"]] = new Dictionary<string, PlantData>();
        }
    }

    public void SetPlantOfType(string json)
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
            plants[plantType][name].plantID = tmpPlant["id"];
        }
    }

    public void SetPlantData(string json)
    {
        var jsonObject = JSONObject.Parse(json);
        var tmp = jsonObject["data"]["getPlant"];
        PlantData plantData = plants[tmp["type"]["name"]][tmp["name"]];
        plantData.plantID = tmp["id"];
        plantData.phRangeLow = tmp["phRangeLow"];
        plantData.phRangeHigh = tmp["phRangeHigh"];
        plantData.rusticity = tmp["rusticity"];
        plantData.sunNeed = tmp["sunNeed"];
        plantData.waterNeed = tmp["sunNeed"];
        StartCoroutine(GetTexture(plantData, tmp["thumbnail"]));
    }

    private IEnumerator GetTexture(PlantData plantData, string imageUrl)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
            Debug.Log(www.error);
        else
            plantData.image = ((DownloadHandlerTexture)www.downloadHandler).texture;
        //callbackFinishDownloadImage.Invoke(plantData.name);
    }
}