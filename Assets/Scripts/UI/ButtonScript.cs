using UnityEngine;
using Doozy.Engine.UI;
using TMPro;

public class ButtonScript : MonoBehaviour
{
    public TextMeshProUGUI plantName;
    public Texture refTexture;

    protected string ghostType;

    public void SetGhostType(string ghostType)
    {
        this.ghostType = ghostType;
    }

    public void OnImageDownload(Texture texture)
    {
        string plantName = this.GetComponent<UIButton>().TextMeshProLabel.text;
        Camera.main.GetComponent<UIController>().dataPanel.SetPlantImg(plantName, this.ghostType, texture);
    }

    public void BuildFunction()
    {
        PlantData tmp = ReactProxy.instance.GetPlantsData(this.ghostType, this.plantName.text);
        if (tmp == null)
            return;
        ConstructionController.instance.SpawnGhost(SpawnController.instance.GetPlantGhost(this.ghostType, tmp.name));
    }

    public void SetPanelFunction()
    {
        ReactProxy.instance.externalData.callbackFinishDownloadImage[this.plantName.text] = OnImageDownload;
        Camera.main.GetComponent<UIController>().SetDataPanel(this.plantName.text, ghostType);
    }
}
