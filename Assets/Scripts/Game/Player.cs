using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject prefBrick;
    [SerializeField] private Transform posBrick;
    [SerializeField] private Stack<GameObject> stackBricks = new();
    private StatePlayer statePlayer;
    private float speed = 0.0f;
    private float posYSpawn = 0.0f;
    private float heightBrick = 0.0f;
    private Vector3 current = Vector3.zero;
    private Vector3 target = Vector3.zero;
    private Vector3 checkBrick = Vector3.zero;

    private Action onGameover;
    private void Start()
    {
        checkBrick = Vector3.forward;
        posYSpawn = this.transform.position.y;
        heightBrick = prefBrick.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.size.z;
    }

    public void Reset()
    {
        if(stackBricks.Count > 0)
        {
            for(int i = 0; i < stackBricks.Count - 1; i++)
            {
                RemoveBrick();
            }    
        }    
    }    

    private void FixedUpdate()
    {
        CheckPrivote();
        Move(statePlayer);
        CheckBrick();
    }

    public void Init(float speed, Action onGameover)
    {
        this.speed = speed;
        this.onGameover = onGameover;
    }

    public void ChangeStatePlayer(StatePlayer statePlayer)
    {
        this.statePlayer = statePlayer;
        switch (statePlayer)
        {
            case StatePlayer.MoveForward:
                checkBrick = Vector3.forward;
                break;
            case StatePlayer.MoveBackward:
                checkBrick = Vector3.back;
                break;
            case StatePlayer.MoveLeft:
                checkBrick = Vector3.left;
                break;
            case StatePlayer.MoveRight:
                checkBrick = Vector3.right;
                break;
        }
    }

    private void Move(StatePlayer statePlayer)
    {
        Vector3 posMoving = Vector3.zero;
        switch (statePlayer)
        {
            case StatePlayer.Idle:
                posMoving = Vector3.zero;
                break;
            case StatePlayer.MoveForward:
                posMoving = Vector3.forward;
                break;
            case StatePlayer.MoveBackward:
                posMoving = Vector3.back;
                break;
            case StatePlayer.MoveLeft:
                posMoving = Vector3.left;
                break;
            case StatePlayer.MoveRight:
                posMoving = Vector3.right;
                break;
        }

        Moving(posMoving);
    }

    public void Moving(Vector3 posMoving)
    {
        current = this.transform.position;
        target = this.transform.position + posMoving;
        this.transform.position = Vector3.MoveTowards(current, target, speed);
    }

    public void CheckPrivote()
    {
        RaycastHit hit;
        Physics.Raycast(this.transform.position + checkBrick, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity);
        Debug.DrawRay(this.transform.position + checkBrick, transform.TransformDirection(Vector3.down), Color.black, Mathf.Infinity);
        if (hit.collider == null)
        {
            statePlayer = StatePlayer.Idle;
        }
        else
        {
            //if(!hit.collider.tag.Contains("Brick") && !hit.collider.tag.Contains("Bridge"))
            if (hit.collider.tag.Contains("Wall"))
                statePlayer = StatePlayer.Idle;
        }
    }

    public void CheckBrick()
    {
        RaycastHit hit;
        Physics.Raycast(this.transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity);
        //Debug.DrawRay(this.transform.position, transform.TransformDirection(Vector3.down), Color.red, Mathf.Infinity);
        if (hit.collider != null)
        {
            if (hit.collider.name == "Brick")
            {
                AddBrick();
                HideBrick(hit.collider.gameObject);
            }
            else if (hit.collider.tag == "Bridge")
            {
                FillBrickOnBridge(hit.collider.GetComponent<Bridge>());
            }
            else if (hit.collider.tag == "Finish")
            {
                onGameover?.Invoke();
            }
        }
    }

    public void HideBrick(GameObject brick)
    {
        DestroyImmediate(brick);
    }

    public void AddBrick()
    {
        Vector3 posSpawn = new Vector3(this.transform.position.x, posYSpawn, this.transform.position.z);
        GameObject brick = SimplePool.Spawn(prefBrick, posSpawn, Quaternion.Euler(-90, 0, 0));
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + heightBrick, this.transform.position.z);
        stackBricks.Push(brick);
        brick.transform.SetParent(posBrick);
    }

    private void FillBrickOnBridge(Bridge bridge)
    {
        if (bridge.IsFill)
            return;
        Transform bridgeTransform = bridge.transform;
        GameObject brick = stackBricks.Peek();
        brick.transform.SetParent(bridgeTransform);
        brick.transform.position = bridgeTransform.position;
        bridge.IsFill = true;
        RemoveBrick();
    }

    private void RemoveBrick()
    {
        stackBricks.Pop();
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - heightBrick, this.transform.position.z);
    }
}
