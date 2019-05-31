using UnityEditor;
using UnityEngine;

public class ModelImporter : MonoBehaviour
{
    public ModelList plantModels;
    public string folderName;
    public bool active = false;
    public string[] colors;

    private Material[][] materials = new Material[10][];

    private void Start()
    {
        if (!active)
            return;
        int count = 0;
        int index = 0;
        foreach (Material elem in Resources.LoadAll(folderName, typeof(Material)))
        {
            if (count == 0)
                materials[index] = new Material[5];
            materials[index][count] = elem;
            count++;
            if (count > 4)
            {
                count = 0;
                index++;
            }
        }
        for (int elemIndex = 0; elemIndex < materials.Length; elemIndex++)
        {
            PlantModel tmp = new PlantModel();
            tmp.materials = new Material[colors.Length];
            tmp.materialsColors = new string[colors.Length];
            for (int colorIndex = 0; colorIndex < 5; colorIndex++)
            {
                tmp.materials[colorIndex] = materials[elemIndex][colorIndex];
                tmp.materialsColors[colorIndex] = colors[colorIndex];
            }
            plantModels.datas.Add(tmp);
        }
        EditorUtility.SetDirty(plantModels);
    }
}
