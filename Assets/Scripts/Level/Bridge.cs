using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    private bool isFill = false;

    public bool IsFill { get => isFill; set => isFill = value; }

    void Start()
    {
        isFill = false;
    }  
}
