using NaughtyAttributes;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
        [SerializeField] private GameObject prefabFinish;
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
            for (int i = 0; i < quantityBlock; i++)
            {
                if (i == quantityBlock - 1)
                    quantitySpawn = totalBricks - (quantityLine * (quantityBlock - 1));
                CreateBrigde(quantitySpawn);
            }
        }

        [Button("Create Map")]
        public void CreateMap()
        {
            Vector3 posSpawnBlock = Vector3.zero;
            Vector3 posFirstBrick = Vector3.zero;
            Vector3 posLastBrick = Vector3.zero;
            float posXBlock = 0;
            float currentZ = 0;
            float posXBridge = 0;

            GameObject map = new GameObject("Map");
            map.AddComponent<Map>();
            int quantityLine = totalBricks / quantityBlock;
            int quantitySpawn = quantityLine;

            for (int i = 0; i < quantityBlock; i++)
            {
                Block blockSpawn = levelBlocks.Dequeue();
                if (i == 0)
                {
                    posXBridge = blockSpawn.LastBrick.x;
                    map.GetComponent<Map>().posPlayer = blockSpawn.FirstBrick;
                }
                if (i > 0)
                {
                    posFirstBrick = blockSpawn.FirstBrick;
                    posXBlock = PositionSpawn(posXBlock, posFirstBrick, posLastBrick);
                    currentZ = posSpawnBlock.z + Math.Abs(maxLimit.x - minLimit.x) + quantitySpawn + 1f;
                    posSpawnBlock = new Vector3(posXBlock, 0, currentZ);
                }
                Debug.Log(currentZ);
                AutomationCreateBlock(blockSpawn.gameObject, map.transform, posSpawnBlock);
                posLastBrick = blockSpawn.LastBrick;
                if (i == quantityBlock - 1)
                {
                    quantitySpawn = blockSpawn.LstBrickBody.Count;
                    //quantitySpawn = totalBricks - (quantityLine * (quantityBlock - 1));
                }
                currentZ += blockSpawn.LastBrick.z + 1;
                posXBridge = posXBlock + blockSpawn.LastBrick.x;
                AutomationCreateBridge(quantitySpawn, map.transform, new Vector3(posXBridge, blockSpawn.LastBrick.y, currentZ));
            }

            currentZ += quantitySpawn;
            Vector3 posFinish = new Vector3(posXBridge, 0, currentZ);
            //Spawn finish block
            GameObject finishBlock = GameObject.Instantiate(prefabFinish, posFinish, Quaternion.identity, map.transform);
        }

        private float PositionSpawn(float posXBlock, Vector3 posFirstBrick, Vector3 posLastBrick)
        {
            float disAC = 0, disBC = 0, distanceAB = 0;
            //disAC = Vector3.Distance(posFirstBrick, new Vector3(posXBlock, posFirstBrick.y, posFirstBrick.z));
            //disBC = Vector3.Distance(posLastBrick, new Vector3(posXBlock, posLastBrick.y, posLastBrick.z));
            //disAC = Math.Abs(posFirstBrick.x);
            //disBC = Math.Abs(posLastBrick.x);
            //if ((posFirstBrick.x < 0 && posLastBrick.x < 0) || (posFirstBrick.x > 0 && posLastBrick.x > 0))
            //{
            //    distanceAB = disAC - disBC;
            //}
            //else
            //{
            //    distanceAB = disAC + disBC;
            //}

            float posXNextBlock = posXBlock + posLastBrick.x - posFirstBrick.x;

            return posXNextBlock;
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