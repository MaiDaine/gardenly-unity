using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalData
{
    public Dictionary<string, string> plantsTypes = new Dictionary<string, string>();
    public Dictionary<string, Dictionary<string, PlantData>> plants = new Dictionary<string, Dictionary<string, PlantData>>();

    public ExternalData(Dictionary<string, Action<string>> callbacks)
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
        plantData.imageUrl = tmp["thumbnail"];
    }
}
