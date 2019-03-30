using UnityEngine;

public class MenuFlowerBedScript : MonoBehaviour
{
    private ConstructionController constructionController;
    private FlowerBed flowerBed;
    
    private void Start()
    {
        this.constructionController = ConstructionController.instance;
    }


    public void DestroyFlowerBedHandler()
    {
        if (constructionController.currentState == ConstructionController.ConstructionState.Off)
        {
           DestroyMenu();
           Destroy(this.flowerBed.gameObject);
           //Destroy(this.flowerBed);
        }
    }

    public void DestroyMenu()
    {
        Destroy(this.gameObject);
        UIController.flowerBedMenuOpen = false;
    }

    public void SetFlowerBedHandler(FlowerBed handler) { this.flowerBed = handler; }
}
