using UnityEngine;
using TMPro;
using Doozy.Engine.UI;

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

    public void UpdateSearch()
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
                        --searchLimit;
                    }
                }
            }
        }
    }
}
