using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantData
{
    public enum DataStatus { None, Requested, Received };

    public string plantID;
    public string name;
    public string description;
    public string maintainAdvice;
    public string periodicity;
    public string soilType;
    public string shape;
    public string[] plantColor;
    public int heightMin;
    public int heightMax;
    public int plantingPeriodBegin;
    public int plantingPeriodEnd;
    public int cuttingPeriodBegin;
    public int cuttingPeriodEnd;
    public int floweringPeriodBegin;
    public int floweringPeriodEnd;
    public int phRangeLow;
    public int phRangeHigh;
    public int waterNeed;
    public int sunNeed;
    public int rusticity;
    public Texture2D image;
    public int model = -1;
    public DataStatus status = DataStatus.None;

    public PlantData(string name) { this.name = name; }

}

