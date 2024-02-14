using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        private Vector3 offset;
        // Start is called before the first frame update
        void Start()
        {
            offset = new Vector3(0, 4, -8);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.fixedDeltaTime * 10);
        }
    }
}
