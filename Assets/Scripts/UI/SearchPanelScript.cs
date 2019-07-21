using UnityEngine;
using TMPro;

public class SearchPanelScript : MonoBehaviour
{
    public Transform searchContent;
    public GameObject prefabButton;
    public TMP_InputField searchText;
    public string[] plantTypes;

    private void ClearSearchContent()
    {
        ButtonScript[] buttons = searchContent.GetComponentsInChildren<ButtonScript>();
        if (buttons.Length > 0)
        {
            foreach(ButtonScript button in buttons)
            {
                Destroy(button.gameObject);
            }
        }
    }

    private void SetButtonPlantData(GameObject obj, string plantType, string plantName)
    {
        PlantPanelScript script = obj.GetComponent<PlantPanelScript>();
        ButtonScript buttonScript = obj.GetComponent<ButtonScript>();

        script.GetData(plantType, plantName);
    }

    private void AddSeparator(GameObject obj)
    {
        obj = new GameObject();
        obj.AddComponent<RectTransform>();
        obj.AddComponent<ButtonScript>();
        Instantiate(obj, searchContent.transform);
        Instantiate(obj, searchContent.transform);
    }


    public void UpdateSearch() // space + better search of str in word see begin...
    {
        GameObject obj = null;
        int searchLimit = 6;

        ClearSearchContent();
        foreach (string type in plantTypes)
        {
            string[] plantNames = ReactProxy.instance.GetPlantsType(type);

            if (plantNames != null)
            {
                foreach (string plantName in plantNames)
                {
                    if (plantName.ToUpper().Contains(searchText.text.ToUpper()) && searchLimit > 0)
                    {
                        obj = Instantiate(prefabButton, searchContent.transform);
                        ButtonScript.SetDynamicButton(obj, type, plantName);
                        SetButtonPlantData(obj, type, plantName);
                        --searchLimit;
                    }
                }
            }
        }
        AddSeparator(obj);
    }
}
