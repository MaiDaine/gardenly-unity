using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class SearchPanelScript : MonoBehaviour
{
    public Transform mainContent;
    public Transform[] searchContent;
    public Transform[] labels;
    public GameObject prefabButton;
    public TMP_InputField searchText;
    public string[] plantTypes;

    private Dictionary<string, List<string>> classifyNames;

    private string FormatString(string stringToFormat)
    {
        return stringToFormat.ToUpper().Normalize(NormalizationForm.FormD);
    }

    private void ClearSearchContent()
    {
        ButtonScript[] buttons = mainContent.GetComponentsInChildren<ButtonScript>();

        if (buttons.Length > 0)
        {
            foreach (ButtonScript button in buttons)
            {
                Destroy(button.gameObject);
            }
        }
        ClearContainers();
    }

    private void SetButtonPlantData(GameObject obj, string plantType, string plantName)
    {
        PlantPanelScript script = obj.GetComponent<PlantPanelScript>();
        ButtonScript buttonScript = obj.GetComponent<ButtonScript>();

        script.GetData(plantType, plantName);
    }

    private int AlphabeticalClassification(string name, string nameToCompare, int index)
    {
        if (name.CompareTo(nameToCompare) < 0)
            return index;
        else
            return index + 1;
    }

    private void ClassifyPlantsName(string type, string name)
    {
        int updateIndex = 0;
        string textRef = FormatString(searchText.text);
       
        if (classifyNames[type].Count == 0)
            classifyNames[type].Add(name);
        else
        {
            for (int i = 0; i < classifyNames[type].Count; i++)
            {
                if (FormatString(name).IndexOf(textRef) > FormatString(classifyNames[type][i]).IndexOf(textRef))
                    updateIndex = i + 1;
                else if (FormatString(name).IndexOf(textRef) < FormatString(classifyNames[type][i]).IndexOf(textRef))
                {
                    updateIndex = i;
                    break;
                }
                else
                    updateIndex = AlphabeticalClassification(FormatString(name), FormatString(classifyNames[type][i]).ToUpper(), updateIndex);
            }

            if (updateIndex >= classifyNames[type].Count)
                classifyNames[type].Add(name);
            else
                classifyNames[type].Insert(updateIndex, name);
        }
    }

    private void ToggleContainers(string type, int typeIndex)
    {
        if (classifyNames[type].Count > 0)
        {
            searchContent[typeIndex].gameObject.SetActive(true);
            labels[typeIndex].gameObject.SetActive(true);
        }
        else
        {
            searchContent[typeIndex].gameObject.SetActive(false);
            labels[typeIndex].gameObject.SetActive(false);
        }
    }

    private void ClearContainers()
    {
        int typeIndex = 0;

        foreach (string type in plantTypes)
        {
            searchContent[typeIndex].gameObject.SetActive(false);
            labels[typeIndex].gameObject.SetActive(false);
            ++typeIndex;
        }
    }

    private void InstantiatePlantsButton()
    {
        GameObject obj = null;
        int searchLimit;
        int typeIndex = 0;

        foreach (string type in classifyNames.Keys)
        {
            searchLimit = 4;
            ToggleContainers(type, typeIndex);

            foreach (string plantName in classifyNames[type])
            {
                if (searchLimit > 0)
                {
                    obj = Instantiate(prefabButton, searchContent[typeIndex].transform);
                    ButtonScript.SetDynamicButton(obj, type, plantName);
                    SetButtonPlantData(obj, type, plantName);
                }
            }
            ++typeIndex;
        }
    }


    public void UpdateSearch()
    {
        classifyNames = new Dictionary<string, List<string>>();
        ClearSearchContent();

        if (searchText.text.Length == 0)
            return;
        foreach (string type in plantTypes)
        {
            if (!classifyNames.ContainsKey(type))
                classifyNames.Add(type, new List<string>());
            string[] plantNames = ReactProxy.instance.GetPlantsType(type);

            if (plantNames != null)
            {
                foreach (string plantName in plantNames)
                {
                    if (FormatString(plantName).Contains(FormatString(searchText.text)))
                        ClassifyPlantsName(type, plantName);
                }
            }
        }
        InstantiatePlantsButton();
    }
}
