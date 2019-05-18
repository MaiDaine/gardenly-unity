using UnityEngine;

public class MenuFlowerBedScript : MonoBehaviour, IMenu
{
    public bool isHidden = false;

    private ConstructionController constructionController;
    private FlowerBed flowerBed;
    
    private void Start()
    {
        this.constructionController = ConstructionController.instance;
    }

    public void DestroyMenu(bool spawn = false)
    {
        UIController uIController = Camera.main.GetComponentInChildren<UIController>();

        if (uIController.flowerBedDataPanel.IsVisible)
            uIController.flowerBedDataPanel.Hide();
            UIController.flowerBedMenuOpen = false;
    }

    public void SetFlowerBedHandler(FlowerBed handler) { this.flowerBed = handler; }

    public GameObject GetGameObject() { return this.gameObject; }

    public void SetHidden(bool state) { this.isHidden = state; }

    public bool IsHidden() { return this.isHidden; }

    public void DestroyObject()
    {
        if (constructionController.currentState == ConstructionController.ConstructionState.Off)
        {
            DestroyMenu();
            Destroy(this.flowerBed.gameObject);
        }
    }
}
