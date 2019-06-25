using UnityEngine;
using TMPro;

public class ButtonScript : MonoBehaviour
{
    public TextMeshProUGUI plantName;

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
        ConstructionController.instance.LoadPlantGhost(SpawnController.instance.GetPlantGhost(ghostType, tmp.name));
    }

    public void SetPanelFunction()
    {
        Camera.main.GetComponent<UIController>().SetDataPanel(plantName.text, ghostType);
    }
}
