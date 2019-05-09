using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphQL
{
    public GraphQL() {}

    public string GetPlantsTypes()
    {   
        return "{\"query\": \"query GetTypes{getTypes{name, id}}\", \"variables\": {}}";
    }

    public string GetPlantsOfType(string name, string typeId)
    {
        return "{\"query\": \"query GetAllPlants($typeId: ID!, $name: String!) {getAllPlants(name: $name,filters:{typeId: $typeId}){" + name + ": edges{node{name, id}}}}\", \"variables\": {\"typeId\": \"" + typeId + "\", \"name\": \"\"}}";
    }
        
    public string GetPlantData(string plantId)
    {
        return "{\"query\": \"query GetPlant($id: ID!) {getPlant(id: $id){type{name}, name, phRangeLow, phRangeHigh, thumbnail, rusticity, sunNeed, waterNeed}}\", \"variables\": {\"id\": \"" + plantId + "\"}}";
    }

}
