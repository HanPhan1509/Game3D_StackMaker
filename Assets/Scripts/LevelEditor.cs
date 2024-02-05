using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Game
{
    public class LevelEditor : MonoBehaviour
    {
        [SerializeField] private Transform map;
        [SerializeField] private GameObject prefabBridge;
        [SerializeField] private int quantityBrick = 10;

        [Space(2f)]
        [Header("PRIVOTE")]
        [SerializeField] private GameObject prefabPrivoteWall;
        [SerializeField] private Vector3 minLimit;
        [SerializeField] private Vector3 maxLimit;

        float widthBridge = 0.0f;
        float widthPrivote = 0.0f;
        void Start()
        {

        }

        [Button("Get value")]
        public void GetValue()
        {
            widthBridge = prefabBridge.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
            widthPrivote = prefabPrivoteWall.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.size.y;
        }    

        [Button("Clear Map")]
        public void ClearMap()
        {
            for (int i = map.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(map.GetChild(i).gameObject);
            }
        }    

        [Button("Create Bridge")]
        public void CreateBrigde()
        {
            float pos = 0;
            //cube z dày
            for (int i = 0; i < quantityBrick; i++)
            {
                GameObject bridge = GameObject.Instantiate(prefabBridge, new Vector3(0, 0, pos), Quaternion.Euler(-90, 0, 0));
                bridge.transform.SetParent(map);
                pos += widthBridge;
            }
        }

        [Button("Create Block")]
        public void CreateBlock()
        {
            for (float x = minLimit.x; x < maxLimit.x; x += widthPrivote)
            {
                for (float z = minLimit.z; z < maxLimit.z; z += widthPrivote)
                {
                    Vector3 pos = new Vector3(x, 0, z);
                    GameObject bridge = GameObject.Instantiate(prefabPrivoteWall, pos, Quaternion.identity);
                    bridge.transform.SetParent(map);
                }    
            }    
        }    
    }
}
