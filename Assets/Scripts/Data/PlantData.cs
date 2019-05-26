using UnityEngine;

public class PlantData
{
    public enum DataStatus { None, Requested, Received };

    //TODO custom user value + serialization

    public string plantID;
    public string type;
    public string name;
    public string description = "La description est absente";
    public string maintainAdvice = "Les conseils d'entretien sont absents"; //
    public string periodicity = "";
    public string soilType = "Absent";
    public string shape = "Absent";
    public string imgUrl = null;
    public string[] plantColor = { "Absent" };
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
}

