using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class OptionPanelScript : MonoBehaviour
{
    [SerializeField] GameObject OptionPanel;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject TestObject;
    [SerializeField] private SelectedButtonUI selectedButtonUI;
    [SerializeField] private SelectedElement selectedElement;
    
    public void OpenOption()
    {
        Debug.Log("ŠJ‚¢‚½‚æ");
        EventSystem.current.SetSelectedGameObject(TestObject);
        OptionPanel.SetActive(true);
        playerInput.SwitchCurrentActionMap("UI");
        selectedElement.RestSlotText();
        Time.timeScale = 0f;
    }

    public void CloseOption()
    {
        Debug.Log("•Â‚¶‚½‚æ");
        OptionPanel.SetActive(false);
        playerInput.SwitchCurrentActionMap("Player");
        Time.timeScale = 1f;
        selectedButtonUI.ResetPosition();        
    }
}
