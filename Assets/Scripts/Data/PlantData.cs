using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantData
{
    public string plantID;
    public string name;
    public string description;
    public int phRangeLow;
    public int phRangeHigh;
    public int waterNeed;
    public int sunNeed;
    public int rusticity;
    public Texture2D image;

    public PlantData(string name) { this.name = name; }

}

