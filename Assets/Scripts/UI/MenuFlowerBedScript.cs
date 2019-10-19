using UnityEngine;
using Doozy.Engine.UI;

// Manage destruction flowerBed panel / object
public class MenuFlowerBedScript : MonoBehaviour, IMenu
{
    public bool isHidden = false;

    private ConstructionController constructionController;
    private FlowerBed flowerBed;

    private void Start()
    {
        constructionController = ConstructionController.instance;
    }


    public void DestroyMenu()
    {
        LabelScript[] scripts = GetComponentsInChildren<LabelScript>();

        if (scripts.Length > 0)
        {
            foreach (LabelScript script in scripts)
                script.ResetColor();
        }
        GetComponent<UIView>().Hide();
    }

    public void DestroyObject()
    {
        if (constructionController.currentState == ConstructionController.ConstructionState.Off)
        {
            DestroyMenu();
            Destroy(flowerBed.gameObject);
        }
    }

    public void SetFlowerBedHandler(FlowerBed handler) { flowerBed = handler; }

    public GameObject GetGameObject() { return gameObject; }

    public void SetHidden(bool state) { isHidden = state; }

    public bool IsHidden() { return isHidden; }
}
