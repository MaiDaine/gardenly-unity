using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.UI;
using TMPro;

public class ButtonScript : MonoBehaviour
{
    public TextMeshProUGUI plantName;
    public GhostHandler[] ghosts;
    public int idxObject;
    public Texture refTexture;

    protected string ghostType;

    private void Update()
    {
    }

    public void OnImageDownload(Texture texture)
    {
        RawImage img = this.GetComponentInChildren<RawImage>();
        img.texture = texture;
        Camera.main.GetComponent<UIController>().SetDataPanel(this.GetComponent<UIButton>().TextMeshProLabel.text, ghostType);
    }

    public void SetGhost(string ghostType)
    {
        if (ghostType == "Arbre")
            idxObject = 1;
        else
            idxObject = 0;
        this.ghostType = ghostType;
    }

    public void BuildFunction()
    {
        PlantData tmp = ReactProxy.instance.GetPlantsData(this.ghostType, this.plantName.text);
        if (tmp == null)
            return;
        this.ghosts[this.idxObject].SetData(tmp);
        ConstructionController.instance.SpawnGhost(this.ghosts[this.idxObject]);
    }

    public void SetPanelFunction()
    {
        string tmp = this.GetComponent<UIButton>().TextMeshProLabel.text;
        ReactProxy.instance.externalData.callbackFinishDownloadImage[tmp] = OnImageDownload;
        Camera.main.GetComponent<UIController>().SetDataPanel(tmp, ghostType);
    }
}
