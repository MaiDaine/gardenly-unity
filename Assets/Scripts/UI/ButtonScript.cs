using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;

public class ButtonScript : MonoBehaviour
{
    public GhostHandler[] ghosts;
    public int idxObject;

    protected string ghostType;

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
