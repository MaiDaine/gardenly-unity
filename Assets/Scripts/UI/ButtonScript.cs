using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public GhostHandler[] ghosts;
    public int idxObject;

    public void SetGhost(GhostHandler ghost, string ghostName = null)
    {
        if (ghostName == null)
        {
            for (int i = 0; i < ghosts.Length; i++)
            {
                if (this.ghosts[i].name == ghost.name || ghost.name == this.ghosts[i].name + "(Clone)")
                    this.idxObject = i;
            }
        }
        else
        {
            for (int i = 0; i < ghosts.Length; i++)
            {
                if (this.ghosts[i].name == ghostName || ghostName == this.ghosts[i].name + "(Clone)")
                    this.idxObject = i;
            }
        }
    }

    public void BuildFunction()
    {
        ConstructionController.instance.SpawnGhost(this.ghosts[this.idxObject]);
    }

    public void SetPanelFunction()
    {
        Camera.main.GetComponent<UIController>().SetDataPanel(this.ghosts[this.idxObject]);
    }
}
