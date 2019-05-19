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

    /*private void Update()
    {
        if (ghostType != null)
        {
            RawImage img = this.GetComponentInChildren<RawImage>();

            if (img != null && img.texture == refTexture && this.GetComponent<UIButton>() != null && ReactProxy.instance.externalData.plants[this.ghostType][this.GetComponent<UIButton>().TextMeshProLabel.text].image != null)
            {
                img.texture = ReactProxy.instance.externalData.plants[this.ghostType][this.GetComponent<UIButton>().TextMeshProLabel.text].image;
            }
        }
    }*/

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
        this.ghosts[this.idxObject].SetData(ReactProxy.instance.externalData.plants[this.ghostType][this.plantName.text]);
        ConstructionController.instance.SpawnGhost(this.ghosts[this.idxObject]);
    }

    public void SetPanelFunction()
    {
        Camera.main.GetComponent<UIController>().SetDataPanel(this.GetComponent<UIButton>().TextMeshProLabel.text, ghostType);
    }
}
