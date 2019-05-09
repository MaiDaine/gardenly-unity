using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantData
{
    public string plantID;
    public string name;
    public string description;
    //image
    public int phRangeLow;
    public int phRangeHigh;
    public int waterNeed;
    public int sunNeed;
    public int rusticity;
    public string imageUrl;

    public PlantData(string name) { this.name = name; }

}

