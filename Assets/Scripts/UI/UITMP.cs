using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//TODO UI
//search all Camera.main.GetComponent<UITMP>() and remplace calls

public class UITMP : MonoBehaviour
{
    //from PlayerController
    public void StartNewShape()
    {
        UIController tmp = Camera.main.GetComponentInChildren<UIController>();//GetComponent ?
        LabelScript labelScript = null;

        if (tmp != null)
            labelScript = tmp.tmpBtn[0].GetComponentInChildren<LabelScript>();
        if (labelScript != null && !labelScript.pressed)
        {
            ConstructionController.instance.Cancel();
            return;
        }
        if (ConstructionController.instance.currentState == ConstructionController.ConstructionState.Off)
        {
            Camera.main.GetComponent<UIController>().Cancel();//tmp.Cancel() ?
            SpawnController.instance.StartNewShape();
        }
    }

    //from PlayerController.DestroySelection()
    public void OnSelectionDestroy()
    {
        UIController uIController = Camera.main.GetComponentInChildren<UIController>();//GetComponent ?
        if (uIController.GetMenuScript() != null)
            uIController.GetMenuScript().DestroyMenu();
        if (uIController.GetFlowerBedMenuScript() != null)
            uIController.GetFlowerBedMenuScript().DestroyMenu();
    }

    //from FlowerBed
    public void SetTutorial()
    {
        if (TutoBoxScript.isOn)
        {
            UIController controller = Camera.main.GetComponent<UIController>();
            controller.plantsViews[5].GetComponentInChildren<TutoBoxScript>().SetTutorial("");
        }
    }


    //from FlowerBedElement
    public void OnPlantElementSelect()
    {
        UIController uIController = Camera.main.GetComponent<UIController>();

        if (uIController.GetMenuScript() != null)
        {
            uIController.GetMenuScript().GetComponentInChildren<LabelScript>().ResetColor();
            uIController.GetMenuScript().DestroyMenu();
        }
        else
            uIController.DestroyMenu();
    }

    public void OnPlantElementDeselect(GhostHandler ghost, string plantName, string plantType)
    {
        UIController uIController = Camera.main.GetComponent<UIController>();
        TextMeshProUGUI[] labels = uIController.dataPanel.GetComponentsInChildren<TextMeshProUGUI>();

        if (ConstructionController.instance.currentState == ConstructionController.ConstructionState.Off)
        {
            RectTransform menuTransform = uIController.extendMenu.RectTransform;
            RectTransform viewTransform = uIController.plantsViews[0].RectTransform;
            uIController.SpawnDynMenu(ghost);
            if (!uIController.PlantsViewsDisplay())
                uIController.dataPanel.GetView().CustomStartAnchoredPosition = new Vector3(-menuTransform.sizeDelta.x + 0.3f, -33.46f, 0);
            else
                uIController.dataPanel.GetView().CustomStartAnchoredPosition = new Vector3(-menuTransform.sizeDelta.x + viewTransform.sizeDelta.x + 0.3f, -33.46f, 0);
            if (labels[labels.Length - 1].text != plantName || uIController.dataPanel.GetView().IsHidden)
                uIController.SetDataPanel(plantName, plantType);
        }
    }
}
