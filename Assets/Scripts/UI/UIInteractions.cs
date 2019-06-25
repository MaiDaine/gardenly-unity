using UnityEngine;
using TMPro;

public class UIInteractions
{
    private UIController uIController = null;

    public void Init()//TODO UI
    {
        uIController = Camera.main.GetComponent<UIController>();
    }

    public void OnSelectDefaultStaticElement(string plantName, string plantType, DefaultStaticElement ghost)
    {
        if (ConstructionController.instance.currentState == ConstructionController.ConstructionState.Off)
        {
            uIController = Camera.main.GetComponent<UIController>();
            uIController.SpawnDynMenu(ghost);
        }
    }

    public void OnDeselectDefaultStaticElement()
    {
        UIController uIController = Camera.main.GetComponent<UIController>();
        MenuScript menuScript = uIController.GetMenuScript();
        if (menuScript != null)
        {
            menuScript.GetComponentInChildren<LabelScript>().ResetColor();
            menuScript.DestroyMenu();
        }
        else
            uIController.DestroyMenu();
    }

    public void OnSelectPlantElement(string plantName, string plantType, PlantElement ghost)
    {
        TextMeshProUGUI[] labels = uIController.dataPanel.GetComponentsInChildren<TextMeshProUGUI>();

        uIController = Camera.main.GetComponent<UIController>();
        if (ConstructionController.instance.currentState == ConstructionController.ConstructionState.Off)
        {
            RectTransform menuTransform = uIController.extendMenu.RectTransform;
            RectTransform viewTransform = uIController.plantsViews[0].RectTransform;

            uIController.SpawnDynMenu(ghost);
            uIController.SetDataPanel(plantName, plantType, true);
            uIController.dataPanel.OnDataLoaded(ghost.GetData());
        }
    }

    public void OnDeSelectPlantElement()
    {
        if (Camera.main != null)
        {
            uIController = Camera.main.GetComponent<UIController>();
            uIController.Cancel();
        }
    }

    public void OnSelectWall(WallHandler ghost, ConstructionController.ConstructionState state)
    {
        uIController = Camera.main.GetComponent<UIController>();

        if (state == ConstructionController.ConstructionState.Off)
            this.uIController.SpawnDynMenu(ghost);

        ghost.GetText().gameObject.SetActive(true);
    }

    public void OnDeselectWall(LineTextHandler text)
    {
        uIController = Camera.main.GetComponent<UIController>();
        MenuScript menuScript = uIController.GetMenuScript();

        if (text != null && text.gameObject != null)
            text.gameObject.SetActive(false);

        uIController.Cancel();
    }

    public void StartNewFB()
    {
        uIController = Camera.main.GetComponent<UIController>();
        if (TutoBoxScript.isOn)
            uIController.plantsViews[5].GetComponentInChildren<TutoBoxScript>().SetTutorial("");

        SpawnController.instance.StartNewShape();
    }
}
