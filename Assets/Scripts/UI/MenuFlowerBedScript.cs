using UnityEngine;

public class MenuFlowerBedScript : MonoBehaviour
{
    private ConstructionController constructionController;
    private FlowerBed flowerBed;
    
    void Start()
    {
        this.constructionController = ConstructionController.instance;
    }

    public void DestroyFlowerBedHandler()
    {
        if (constructionController.currentState == ConstructionController.ConstructionState.Off)
        {
            DestroyMenu();
            /*foreach (FlowerBedMesh mesh in this.flowerBed.GetMeshes())
            {
                Destroy(mesh);
            }*/
           Destroy(this.flowerBed.gameObject);
           Destroy(this.flowerBed);
        }
    }

    public void DestroyMenu()
    {
        if (this.gameObject)
            Destroy(this.gameObject);
        UIController.flowerBedMenuOpen = false;
        /*if (this.constructionController.currentState == ConstructionController.ConstructionState.Editing
            && flowerBed != null)
            this.CombineMesh();*/
    }

    public void SetFlowerBedHandler(FlowerBed handler)
    {
        this.flowerBed = handler;
    }

    /*public void CombineMesh()
    {
        this.flowerBed.CombineMesh();
    }

    public void AddFlowerBedMesh()
    {
        this.flowerBed.SpawnMesh();
        this.constructionController.currentState = ConstructionController.ConstructionState.Building;
    }*/
}
