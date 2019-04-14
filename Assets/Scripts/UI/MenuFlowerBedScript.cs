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

    private void LateUpdate()
    {
        Quaternion rotation;
        Vector3 relativePos;

        relativePos = this.transform.position - Camera.main.transform.position;
        rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        this.transform.rotation = rotation;
    }

    public void DestroyMenu()
    {
        Destroy(this.gameObject);
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
