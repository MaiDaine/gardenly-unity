using UnityEngine;
using Doozy.Engine.UI;
using TMPro;

public class ButtonScript : MonoBehaviour
{
    public TextMeshProUGUI plantName;

    protected string ghostType;

    public void SetGhostType(string type)
    {
        ghostType = type;
    }

    public void OnImageDownload(Texture texture)
    {
        string plantName = GetComponent<UIButton>().TextMeshProLabel.text;
        Camera.main.GetComponent<UIController>().dataPanel.SetPlantImg(plantName, ghostType, texture);
    }

    //Function call onDynamicButtonClick

    public void BuildFunction()
    {
        PlantData tmp = ReactProxy.instance.GetPlantsData(ghostType, plantName.text);
        if (tmp == null)
            return;
        ConstructionController.instance.SpawnGhost(SpawnController.instance.GetPlantGhost(ghostType, tmp.name));
    }

    public void SetPanelFunction()
    {
        ReactProxy.instance.externalData.callbackFinishDownloadImage[plantName.text] = OnImageDownload;
        Camera.main.GetComponent<UIController>().SetDataPanel(plantName.text, ghostType);
    }
}
