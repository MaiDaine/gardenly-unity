using UnityEngine;

public class MenuFlowerBedScript : MonoBehaviour
{
    private ConstructionController constructionController;
    private FlowerBedHandler flowerBedHandler;
    
    void Start()
    {
        this.constructionController = ConstructionController.instance;
    }

    public void DestroyFlowerBedHandler()
    {
        if (constructionController.currentState == ConstructionController.ConstructionState.Off)
        {
            DestroyMenu();
            foreach (FlowerBedMesh mesh in this.flowerBedHandler.GetMeshes())
            {
                Destroy(mesh);
            }
           Destroy(this.flowerBedHandler.gameObject);
           Destroy(this.flowerBedHandler);
        }
    }

    public void DestroyMenu()
    {
        if (this.gameObject)
            Destroy(this.gameObject);
        UIController.flowerBedMenuOpen = false;
        if (this.constructionController.currentState == ConstructionController.ConstructionState.Editing
            && flowerBedHandler != null)
            this.CombineMesh();
    }

    public void SetFlowerBedHandler(FlowerBedHandler handler)
    {
        this.flowerBedHandler = handler;
    }

    public void CombineMesh()
    {
        this.flowerBedHandler.CombineMesh();
    }

    public void AddFlowerBedMesh()
    {
        this.flowerBedHandler.SpawnMesh();
        this.constructionController.currentState = ConstructionController.ConstructionState.Building;
    }
}
