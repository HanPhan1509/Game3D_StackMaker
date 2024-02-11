using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    [SerializeField] private float speedMoving = 0.5f;

    public float SpeedMoving { get => speedMoving; set => speedMoving = value; }
}
