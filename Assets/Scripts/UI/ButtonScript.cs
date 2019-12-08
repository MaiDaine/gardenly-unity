using UnityEngine;
using TMPro;
using Doozy.Engine.UI;

public class ButtonScript : MonoBehaviour
{
    public TextMeshProUGUI plantName;

    protected PlantData plantData = null;
    protected string ghostType;

    public void SetGhostType(string type)
    {
        ghostType = type;
    }

    //Function call onDynamicButtonClick

    public void BuildFunction()
    {
        PlantData tmp = ReactProxy.instance.GetPlantsData(ghostType, plantName.text);
        if (tmp == null)
            return;
        ConstructionController.instance.SetGhost(null);
        ConstructionController.instance.LoadPlantGhost(SpawnController.instance.GetPlantGhost(tmp));
    }

    public void SetPanelFunction()
    {
        Camera.main.GetComponent<UIController>().SetDataPanel(plantName.text, ghostType);
    }

    public void SetPlantData(PlantData data)
    {
        plantData = data;
    }

    public PlantData GetPlantData()
    {
        return plantData;
    }

    public static void SetDynamicButton(GameObject obj, string plantType, string plantName)
    {
        ButtonScript buttonScript = obj.GetComponent<ButtonScript>();
        UIButton btn = obj.GetComponent<UIButton>();

        buttonScript.SetGhostType(plantType);
        btn.TextMeshProLabel.text = plantName;
    }
}
