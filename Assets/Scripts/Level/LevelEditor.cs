using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public class Privote
    {
        public int id;
        public GameObject privote;
        public Vector3 position;
    }
    public class LevelEditor : MonoBehaviour
    {
        [SerializeField] private Transform bridge;
        [SerializeField] private GameObject prefabBridge;
        [SerializeField] private Block block;
        [SerializeField] private int quantityBrick = 10;

        [Space(2f)]
        [Header("PRIVOTE")]
        [SerializeField] private GameObject prefabPrivoteWall;
        [SerializeField] private GameObject prefabPrivoteBrick;
        [SerializeField] private Vector3 minLimit;
        [SerializeField] private Vector3 maxLimit;
        [SerializeField] private float minQuantityPrivote;
        [SerializeField] private float maxQuantityPrivote;

        private float widthBridge = 0.0f;
        private float widthPrivote = 0.0f;


        [Button("Get value")]
        public void GetValue()
        {
            widthBridge = prefabBridge.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
            //SimplePool.Preload(prefabPrivoteWall, 200);
            widthPrivote = prefabPrivoteWall.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.size.y;
            minQuantityPrivote = Math.Abs(maxLimit.x - minLimit.x);
            maxQuantityPrivote = minQuantityPrivote * 3f;
            block.Init(minLimit, maxLimit, minQuantityPrivote, maxQuantityPrivote, widthPrivote);
        }

        [Button("Clear Map")]
        public void ClearMap()
        {
            block.ClearBlock();
            for (int i = bridge.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(bridge.GetChild(i).gameObject);
            }
        }

        [Button("Create Bridge")]
        public void CreateBrigde()
        {
            float pos = 0;
            for (int i = 0; i < quantityBrick; i++)
            {
                GameObject bri = GameObject.Instantiate(prefabBridge, new Vector3(0, 0, pos), Quaternion.Euler(-90, 0, 0));
                bri.transform.SetParent(bridge);
                pos += widthBridge;
            }
        }

        [Space(2f)]
        [Header("MAP SETTINGS")]
        [SerializeField] private Transform map;
        [SerializeField] private List<Block> blocks;

        [Button("Load Blocks")]
        public void LoadBlock()
        {
            string folderPath = "Assets/Prefabs/Blocks";
            string[] prefabPaths = AssetDatabase.FindAssets("Block_", new[] { folderPath });
            // Lặp qua mỗi đường dẫn Prefab và in ra tên của nó
            foreach (string prefabPath in prefabPaths)
            {
                string prefabFullPath = AssetDatabase.GUIDToAssetPath(prefabPath);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFullPath);
                blocks.Add(prefab.GetComponent<Block>());

                //if (prefab != null)
                //{
                //    Debug.Log("Prefab: " + prefab.name + " - Path: " + prefabFullPath);
                //}
            }
        }    
    }
}