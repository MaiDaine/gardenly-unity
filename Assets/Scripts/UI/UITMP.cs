using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
