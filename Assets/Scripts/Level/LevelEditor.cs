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
        [SerializeField] private GameObject prefabBridge;
        [SerializeField] private Block block;

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
            //for (int i = bridge.childCount - 1; i >= 0; i--)
            //{
            //    DestroyImmediate(bridge.GetChild(i).gameObject);
            //}
        }

        public void CreateBrigde(int quantitySpawn)
        {
            float pos = 0;
            GameObject bridge = new GameObject("Bridge");
            for (int i = 0; i < quantitySpawn; i++)
            {
                GameObject bri = GameObject.Instantiate(prefabBridge, new Vector3(0, 0, pos), Quaternion.Euler(-90, 0, 0));
                bri.transform.SetParent(bridge.transform);
                pos += widthBridge;
            }
            bridges.Enqueue(bridge);
        }

        [Space(2f)]
        [Header("MAP SETTINGS")]
        [SerializeField] private List<Block> blocks;
        [SerializeField] private Queue<Block> levelBlocks = new();
        [SerializeField] private Queue<GameObject> bridges = new();
        [SerializeField] private int quantityBlock = 3;
        [SerializeField] private int totalBricks = 0;

        [Button("Load Blocks")]
        public void LoadBlock()
        {
            string folderPath = "Assets/Prefabs/Blocks";
            string[] prefabPaths = AssetDatabase.FindAssets("Block_", new[] { folderPath });
            
            foreach (string prefabPath in prefabPaths)
            {
                string prefabFullPath = AssetDatabase.GUIDToAssetPath(prefabPath);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFullPath);
                blocks.Add(prefab.GetComponent<Block>());
            }
        }

        [Button("Get Blocks")]
        private void GetBlocksInLevel()
        {
            totalBricks = 0;
            for (int i = 0; i < quantityBlock; i++)
            {
                Block block = blocks[UnityEngine.Random.Range(0, blocks.Count)];
                totalBricks += (block.LstBrickBody.Count + 2);
                levelBlocks.Enqueue(block);
            }
            //GetBridgeInLevel();
        }

        private void GetBridgeInLevel()
        {
            int quantityLine = totalBricks / quantityBlock;
            int quantitySpawn = quantityLine;
            for(int i = 0; i < quantityBlock; i++)
            {
                if (i == quantityBlock - 1)
                    quantitySpawn = totalBricks - (quantityLine * (quantityBlock - 1));
                CreateBrigde(quantitySpawn);
            }    
        }

        [Button("Create Map")]
        public void CreateMap()
        {
            GameObject map = new GameObject("Map");
            map.AddComponent<Map>();
            Vector3 posSpawnBlock = Vector3.zero;
            int quantityLine = totalBricks / quantityBlock;
            int quantitySpawn = quantityLine;

            for (int i = 0; i < quantityBlock; i++)
            {
                Block blockSpawn = levelBlocks.Dequeue();
                //if (i == 0)
                //    map.GetComponent<Map>().posPlayer = blockSpawn.FirstBrick;
                AutomationCreateBlock(blockSpawn.gameObject, map.transform, posSpawnBlock);
                if (i == quantityBlock - 1)
                    quantitySpawn = totalBricks - (quantityLine * (quantityBlock - 1));
                AutomationCreateBridge(quantitySpawn, map.transform, new Vector3(blockSpawn.LastBrick.x, blockSpawn.LastBrick.y, posSpawnBlock.z + blockSpawn.LastBrick.z + 1));
                Debug.Log($"Result: {blockSpawn.FirstBrick.x} - {blockSpawn.LastBrick.x} = {blockSpawn.FirstBrick.x + blockSpawn.LastBrick.x}");
                posSpawnBlock = new Vector3(blockSpawn.FirstBrick.x + blockSpawn.LastBrick.x, 0, posSpawnBlock.z + Math.Abs(maxLimit.x - minLimit.x) + quantitySpawn + 1f);
            }

            //map.GetComponent<Map>().posPlayer = block.FirstBrick;
        }

        public void AutomationCreateBlock(GameObject prefab, Transform parent, Vector3 position)
        {
            Block block = GameObject.Instantiate(prefab, position, Quaternion.identity, parent).GetComponent<Block>();
        }

        public void AutomationCreateBridge(int quantitySpawn, Transform parent, Vector3 position)
        {
            float pos = 0;
            GameObject bridge = new GameObject("Bridge");
            for (int i = 0; i < quantitySpawn; i++)
            {
                GameObject bri = GameObject.Instantiate(prefabBridge, new Vector3(0, 0, pos), Quaternion.Euler(-90, 0, 0));
                bri.transform.SetParent(bridge.transform);
                pos += widthBridge;
            }
            bridge.transform.SetParent(parent.transform);
            bridge.transform.position = new Vector3(position.x, 2.5f, position.z);
        }    
    }
}