using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    [Tooltip("Should match size with Player Behaviors")][SerializeField] private TMP_Text[] _playerTexts;
    [Tooltip("Should match size with Player Texts")][SerializeField] private Player_Behavior[] _playerBehaviors;
    private string _healthText = "x ";


    private void Start()
    {
        SetUpUI();
    }


    public void UpdateLive(Player_Behavior p)
    {
        int playerIndex = GetPlayerIndex(p);
        if (playerIndex != -1)
        {
            _healthText = "x " + _playerBehaviors[playerIndex]._health;
            _playerTexts[playerIndex].SetText(_healthText);
        }
    }
    private void SetUpUI()
    {
        for (int i = 0; i < _playerTexts.Length; i++)
        {
            _healthText = "x " + _playerBehaviors[i]._health;
            _playerTexts[i].SetText(_healthText);
        }
    }
    private int GetPlayerIndex(Player_Behavior target)
    {
        for (int i = 0; i < _playerBehaviors.Length; i++)
            if (_playerBehaviors[i] == target)
                return i;
        return -1;
    }
}
