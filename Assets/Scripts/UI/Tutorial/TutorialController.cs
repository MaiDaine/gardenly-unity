using UnityEngine;
using Doozy.Engine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TutorialController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UIView[] tutoSubMenu;
    public UIButton[] navButtons;
    public TextMeshProUGUI title;
    public TextMeshProUGUI body;

    private bool hoverTuto = false;
    private bool tutoActivate = false;
    private TutoObject tutoObject = null;

    private void Update()
    {
        /*if (tutoActivate)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject() && !hoverTuto)
                {
                    Debug.Log("CLICK ON UI");
                }
            }
        }*/
    }

    //Play Tutorial management
    public void SetTutorial(TutoObject tutorial)
    {
        tutoObject = tutorial;
        tutoObject.progressIndex = 0;
        title.text = tutoObject.GetComponent<UIButton>().TextMeshProLabel.text;
        body.text = tutoObject.instructions[tutoObject.progressIndex];
        navButtons[1].DisableButton();
    }

    public void PlayTutorial()
    {
        tutoObject.PlayTutorial();
        body.text = tutoObject.instructions[tutoObject.progressIndex];
        if (tutoObject.progressIndex + 1 >= tutoObject.instructions.Length)
            navButtons[2].DisableButton();
        if (!navButtons[1].Interactable && tutoObject.progressIndex > 0)
            navButtons[1].EnableButton();
    }

    public void PreviousTutorial()
    {
        tutoObject.PreviousTutorial();
        body.text = tutoObject.instructions[tutoObject.progressIndex];
        if (tutoObject.progressIndex <= 0)
            navButtons[1].DisableButton();
        if (!navButtons[2].Interactable && tutoObject.progressIndex < tutoObject.instructions.Length)
            navButtons[2].EnableButton();
    }

    public void CloseTutorial()
    {
        if (tutoObject != null && tutoObject.buttons.Length > 0)
        {
            tutoObject.buttons[0].ExecuteClick();
        }
    }

    //UI management
    public void CloseSubMenu()
    {
        foreach (UIView view in tutoSubMenu)
        {
            if (view.IsVisible)
                view.Hide();
        }
    }

    public void ToogleNavButtons()
    {
        foreach (UIButton button in navButtons)
        {
            button.gameObject.SetActive(!button.IsActive());
        }
    }

    public void DisableNavButton(UIButton button)
    {

    }

    public void ToogleTutoState()
    {
        tutoActivate = !tutoActivate;
    }

    public bool GetTutoState()
    {
        return tutoActivate;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverTuto = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverTuto = false;
    }
}
