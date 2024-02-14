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

    public void ShowPopup(bool isNext = true)
    {
        buttonNext.gameObject.SetActive(isNext);
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
