using UnityEngine;

public class PlantData
{
    public enum DataStatus { None, Requested, Received };

    //TODO custom user value + serialization

    public string plantID;
    public string type;
    public string name;
    public string description;
    public string maintainAdvice;
    public string periodicity;
    public string soilType;
    public string shape;
    public string imgUrl = null;
    public string[] plantColor;
    public int heightMin = 0;
    public int heightMax = 0;
    public int plantingPeriodBegin = 0;
    public int plantingPeriodEnd = 0;
    public int cuttingPeriodBegin = 0;
    public int cuttingPeriodEnd = 0;
    public int floweringPeriodBegin = 0;
    public int floweringPeriodEnd = 0;
    public int phRangeLow = 0;
    public int phRangeHigh = 0;
    public int waterNeed = 0;
    public int sunNeed = 0;
    public int rusticity = 0;
    public Texture2D image;
    public int model = -1;
    public DataStatus status = DataStatus.None;

    public PlantData(string name) { this.name = name; }

    public void SetDefaultData()
    {
        description = LocalisationController.instance.GetText("plant_data", "description");
        maintainAdvice = LocalisationController.instance.GetText("plant_data", "advices");
        soilType = LocalisationController.instance.GetText("plant_data", "missing");
        shape = LocalisationController.instance.GetText("plant_data", "missing");
    }

    public void PrintState() {
        Debug.Log("--- " + name + " ---");
        Debug.Log("ID : " + plantID);
        Debug.Log("Type : " + type);
        Debug.Log("Description : " + description);
        Debug.Log("Image URL : " + imgUrl);
        Debug.Log("Plant Color :");
        foreach(string color in plantColor)
            Debug.Log(" - " + color);
        Debug.Log("---------------");
    }
}

