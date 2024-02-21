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

        [Space(2f)]
        [Header("PRIVOTE")]
        [SerializeField] private Vector3 minLimit;
        [SerializeField] private Vector3 maxLimit;
        [SerializeField] private int numberMap;

        private float widthBridge = 0.0f;

        [Button("Automation Create Map")]
        public void AutomationCreateMap()
        {
            ClearMap();
            GetValue();
            string folderPath = "Assets/Resources";
            string[] prefabPaths = AssetDatabase.FindAssets("Map_", new[] { folderPath });
            for (int i = prefabPaths.Length; i < prefabPaths.Length + numberMap; i++)
            {
                LoadBlock();
                GetBlocksInLevel();
                CreateMap();
                SaveMap(i + 1);
                ClearMap();
            }
        }

        [Button("Clear Map")]
        public void ClearMap()
        {
            GameObject findMap = GameObject.Find("Map");
            if (findMap != null)
                DestroyImmediate(findMap);
        }


        //[Button("Get value")]
        public void GetValue()
        {
            widthBridge = prefabBridge.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
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
        //[SerializeField] private int totalBricks = 0;

        //[Button("Load Blocks")]
        public void LoadBlock()
        {
            blocks.Clear();
            string folderPath = "Assets/Prefabs/Blocks";
            string[] prefabPaths = AssetDatabase.FindAssets("Block_", new[] { folderPath });

            foreach (string prefabPath in prefabPaths)
            {
                string prefabFullPath = AssetDatabase.GUIDToAssetPath(prefabPath);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFullPath);
                blocks.Add(prefab.GetComponent<Block>());
            }
        }

        //[Button("Get Blocks")]
        public void GetBlocksInLevel()
        {
            //totalBricks = 0;
            for (int i = 0; i < quantityBlock; i++)
            {
                Block block = blocks[UnityEngine.Random.Range(0, blocks.Count)];
                //totalBricks += (block.LstBrickBody.Count + 2);
                levelBlocks.Enqueue(block);
            }
            //GetBridgeInLevel();
        }

        //private void GetBridgeInLevel()
        //{
        //    int quantityLine = totalBricks / quantityBlock;
        //    int quantitySpawn = quantityLine;
        //    for (int i = 0; i < quantityBlock; i++)
        //    {
        //        if (i == quantityBlock - 1)
        //            quantitySpawn = totalBricks - (quantityLine * (quantityBlock - 1));
        //        CreateBrigde(quantitySpawn);
        //    }
        //}

        //[Button("Create Map")]
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
            //int quantityLine = totalBricks / quantityBlock;
            int quantityLine = 0;
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
                    posXBlock = posXBlock + posLastBrick.x - posFirstBrick.x; ;
                    currentZ = posSpawnBlock.z + Math.Abs(maxLimit.x - minLimit.x) + quantitySpawn + 1f;
                    posSpawnBlock = new Vector3(posXBlock, 0, currentZ);
                }
                AutomationCreateBlock(blockSpawn.gameObject, map.transform, posSpawnBlock);
                posLastBrick = blockSpawn.LastBrick;
                //if (i == quantityBlock - 1)
                //{
                //    quantitySpawn = blockSpawn.LstBrickBody.Count + 2;
                //    //quantitySpawn = totalBricks - (quantityLine * (quantityBlock - 1));
                //}
                quantitySpawn = blockSpawn.LstBrickBody.Count + 2;
                currentZ += blockSpawn.LastBrick.z + 1;
                posXBridge = posXBlock + blockSpawn.LastBrick.x;
                AutomationCreateBridge(quantitySpawn, map.transform, new Vector3(posXBridge, blockSpawn.LastBrick.y, currentZ));
            }

            currentZ += quantitySpawn;
            Vector3 posFinish = new Vector3(posXBridge, 0, currentZ);
            //Spawn finish block
            GameObject finishBlock = GameObject.Instantiate(prefabFinish, posFinish, Quaternion.identity, map.transform);
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

        //[Button("Save Map")]
        public static void SaveMap(int numberSave = -1)
        {
            int numberMap = 0;
            string folderPath = "Assets/Resources";
            string[] prefabPaths = AssetDatabase.FindAssets("Map_", new[] { folderPath });

            //GameObject selectedObject = Selection.activeGameObject;
            GameObject selectedObject = GameObject.Find("Map");

            if (numberSave > -1) numberMap = numberSave;
            else numberMap = prefabPaths.Length + 1;

            string prefabPath = "Assets/Resources/Map_" + numberMap.ToString() + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(selectedObject, prefabPath);
        }
    }
}