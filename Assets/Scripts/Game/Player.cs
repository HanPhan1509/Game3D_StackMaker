using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float speed = 0.0f;
    private StatePlayer statePlayer;
    private Vector3 posMoving;
    private Vector3 current = Vector3.zero;
    private Vector3 target = Vector3.zero;


    private void FixedUpdate()
    {
        CheckBrick();
        Move(statePlayer);
    }

    public void Init(float speed)
    {
        this.speed = speed;
    }

    public void ChangeStatePlayer(StatePlayer statePlayer)
    {
        this.statePlayer = statePlayer;
    }

    private void Move(StatePlayer statePlayer)
    {
        switch (statePlayer)
        {
            case StatePlayer.Idle:
                posMoving = Vector3.zero;
                this.transform.position = Vector3.MoveTowards(this.transform.position, this.transform.position + posMoving, speed);
                break;
            case StatePlayer.MoveForward:
                posMoving = Vector3.forward;
                current = this.transform.position;
                target = this.transform.position + posMoving;
                Moving(current, target);
                break;
        }
    }

    public void Moving(Vector3 current, Vector3 target)
    {
        Debug.Log($"Hann null {current} >< {target} >< speed: {speed}");
        this.transform.position = Vector3.MoveTowards(current, target, speed);
    }

    public void CheckBrick()
    {
        RaycastHit hit;
        bool isBrick = Physics.Raycast(this.transform.position + Vector3.forward, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity);
        Debug.DrawRay(this.transform.position + Vector3.forward, transform.TransformDirection(Vector3.down), Color.black, Mathf.Infinity);
        //Debug.Log($"HAN 1 {hit.collider.name}");
        if (hit.collider != null)
        {
            if (!hit.collider.name.Contains("Brick"))
            {
                Debug.Log("Hann collider " + hit.collider.name);
                statePlayer = StatePlayer.Idle;
            }
        }
        else
        {
            statePlayer = StatePlayer.Idle;
        }
    }
}
