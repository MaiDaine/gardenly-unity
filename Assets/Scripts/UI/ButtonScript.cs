using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.UI;

public class ButtonScript : MonoBehaviour
{
    public GhostHandler[] ghosts;
    public int idxObject;

    protected string ghostType;

    private void Update()
    {
        if (ghostType != null)
        {
            RawImage img = this.GetComponentInChildren<RawImage>();
            if (img != null && img.texture == null && this.GetComponent<UIButton>() != null)
            {
                img.texture = ReactProxy.instance.externalData.plants[this.ghostType][this.GetComponent<UIButton>().TextMeshProLabel.text].image;
            }
        }
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
        ConstructionController.instance.SpawnGhost(this.ghosts[this.idxObject]);
    }

    public void SetPanelFunction()
    {
        Camera.main.GetComponent<UIController>().SetDataPanel(this.GetComponent<UIButton>().TextMeshProLabel.text, ghostType);
    }
}
