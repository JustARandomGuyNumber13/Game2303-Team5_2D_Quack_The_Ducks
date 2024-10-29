using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

public class KeyBind_Manager : MonoBehaviour
{
    [SerializeField] private InputActionReference _action;
    [SerializeField] private TMP_Text _keyBindingText;
    [SerializeField] private GameObject _rebindingCanvas;

    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;
    private string _text;


    /* Monobehavior method */
    private void Awake()
    {
        UpdateText();
    }


    /* Rebinding handler methods */
    public void AssignNewBinding()
    {
        _rebindingCanvas.SetActive(true);
        _rebindingOperation = _action.action.PerformInteractiveRebinding(0)
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete())
            .Start();
    }
    private void RebindComplete()
    {
        _rebindingCanvas.SetActive(false);
        UpdateText();
        _rebindingOperation.Dispose();
    }
    private void UpdateText()
    {
        _text = _action.action.GetBindingDisplayString();
        _keyBindingText.text = _text;
    }
}