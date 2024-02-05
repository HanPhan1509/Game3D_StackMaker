using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    public class LevelEditor : MonoBehaviour
    {
        [SerializeField] private Transform map;
        [SerializeField] private GameObject prefabBridge;
        [SerializeField] private int quantityBrick = 10;
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

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
            float widthBridge = 0;
            widthBridge = prefabBridge.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
            //cube z dày
            for (int i = 0; i < quantityBrick; i++)
            {
                GameObject bridge = GameObject.Instantiate(prefabBridge, new Vector3(0, 0, pos), Quaternion.Euler(-90, 0, 0));
                bridge.transform.SetParent(map);
                pos += widthBridge;
            }
        }    
    }
}
