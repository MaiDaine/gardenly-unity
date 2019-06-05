using System;
using UnityEngine;

[Serializable]
public class PlantModel
{
    /*
    public Material[] materials;
    public string[] materialsColors;
    public Material GetModelMaterial(string[] colors)
    {
        int index = 0;
        for (int i = 0; i < materialsColors.Length; i++)
            for (int j = 0; j < colors.Length; j++)
                if (materialsColors[i] == colors[j])
                {
                    index = i;
                    break;
                }
        return materials[index];
    }

    public FlowerBedElement CreateElement(FlowerBedElement model, string[] colors)
    {
        int index = 0;
        for (int i = 0; i < materialsColors.Length; i++)
            for (int j = 0; j < colors.Length; j++)
                if (materialsColors[i] == colors[j])
                {
                    index = i;
                    break;
                }
        model.GetComponent<MeshRenderer>().material = materials[index];
        return model;
    }

    private void RandomizeArray(ref string[] arr)
    {
        for (var i = arr.Length - 1; i > 0; i--)
        {
            var r = UnityEngine.Random.Range(0, i);
            var tmp = arr[i];
            arr[i] = arr[r];
            arr[r] = tmp;
        }
    }*/
}