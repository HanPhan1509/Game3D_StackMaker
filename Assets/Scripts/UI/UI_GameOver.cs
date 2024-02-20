using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_GameOver : MonoBehaviour
{
    [SerializeField] private Button buttonNext;
    [SerializeField] private UnityEvent OnButtonNext;
    [SerializeField] private UnityEvent OnButtonReplay;

    public void ShowPopup(bool isMaxLevel = false)
    {
        buttonNext.gameObject.SetActive(!isMaxLevel);
    }    
    public void ButtonReplay()
    {
        OnButtonReplay?.Invoke();
    }

    public void ButtonNext()
    {
        OnButtonNext?.Invoke();
    }    
}
