using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public GhostHandler[] ghosts;
    //public FlowerBed[] flowerBeds;
    public int idxObject;

    public void SetGhost(GhostHandler ghost)
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            if (this.ghosts[i].name == ghost.name ||  ghost.name == this.ghosts[i].name + "(Clone)")
                this.idxObject = i;
        }
    }

    /*public void SetFlowerBed(FlowerBed flowerBed)
    {

    }*/

    public void BuildFunction()
    {
        ConstructionController.instance.SpawnGhost(this.ghosts[this.idxObject]);
    }

    /*public void SpawnFlowerBed()
    {
        PlayerController.instance.SpawnFlowerBedMesh();
    }*/
}
