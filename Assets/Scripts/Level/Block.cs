using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private Vector3 firstBrick;
        [SerializeField] private Vector3 lastBrick;
        [SerializeField] private List<Vector3> lstBrickBody;

        [Space(2f)]
        [Header("GAMEOBJECT")]
        [SerializeField] private GameObject prefabPrivoteWall;
        [SerializeField] private GameObject prefabPrivoteBrick;

        [Space(2f)]
        [Header("TRANSFORM")]
        [SerializeField] private Transform blockTransform;

        [Space(2f)]
        [Header("PRIVOTE")]
        [SerializeField] private Vector3 minLimit;
        [SerializeField] private Vector3 maxLimit;

        [SerializeField] private float minQuantityPrivote;
        [SerializeField] private float maxQuantityPrivote;
        private float widthPrivote;

        private List<Privote> lstPrivoteDespawn = new();
        private List<Privote> lstPrivote = new();

        public Vector3 FirstBrick { get => firstBrick; set => firstBrick = value; }
        public Vector3 LastBrick { get => lastBrick; set => lastBrick = value; }
        public List<Vector3> LstBrickBody { get => lstBrickBody; set => lstBrickBody = value; }

        [Button("Get Values")]
        public void GetValues()
        {
            widthPrivote = prefabPrivoteWall.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.size.y;
            minQuantityPrivote = Math.Abs(maxLimit.x - minLimit.x);
            maxQuantityPrivote = minQuantityPrivote * 3f;
        }

        [Button("Clear Block")]
        public void ClearBlock()
        {
            lstPrivote.Clear();
            lstBrickBody.Clear();
            lstPrivoteDespawn.Clear();
            for (int i = blockTransform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(blockTransform.GetChild(i).gameObject);
            }
        }

        [Button("Create Block")]
        public void CreateBlock()
        {
            int id = 0;
            for (float x = minLimit.x; x <= maxLimit.x; x += widthPrivote)
            {
                for (float z = minLimit.z; z <= maxLimit.z; z += widthPrivote)
                {
                    Vector3 pos = new Vector3(x, 0, z);
                    GameObject privoteWall = GameObject.Instantiate(prefabPrivoteWall, pos, Quaternion.identity);
                    privoteWall.transform.SetParent(blockTransform);
                    Privote privote = new Privote();
                    privote.privote = privoteWall;
                    privote.position = pos;
                    privote.id = id;
                    lstPrivote.Add(privote);
                    id++;
                }
            }
            FindLineBrick();
        }

        [Button("Create Line Brick")]
        public void FindLineBrick()
        {
            firstBrick = SpawnFirstPrivoteBrick();
            SpawnBrick(firstBrick);

            do
            {
                lstBrickBody.Clear();
                Vector3 nextPos = firstBrick;
                Vector3 currentPos = firstBrick;
                Vector3 beforePos = firstBrick;
                while (nextPos.z != maxLimit.z)
                {
                    nextPos = AutomationDirectionBrick(beforePos, currentPos);
                    lstPrivoteDespawn.Add(lstPrivote.Find(x => x.position == nextPos));
                    lstBrickBody.Add(nextPos);
                    beforePos = currentPos;
                    currentPos = nextPos;
                }
            } while (lstBrickBody.Count > maxQuantityPrivote || lstBrickBody.Count < minQuantityPrivote);

            HashSet<Vector3> uniqueSet = new HashSet<Vector3>(lstBrickBody);
            lstBrickBody.Clear();
            lstBrickBody.AddRange(uniqueSet);

            SpawnLastBrick();
            DestroyLineBrick();
        }

        private void SpawnLastBrick()
        {
            lastBrick = lstBrickBody[lstBrickBody.Count - 1];
            lstBrickBody.RemoveAt(lstBrickBody.Count - 1);
            DestroyImmediate(lstPrivote.Find(x => x.position == lastBrick).privote);
            SpawnBrick(lastBrick);
        }    

        private Vector3 SpawnFirstPrivoteBrick()
        {
            List<int> lstStartPrivote = new();

            //Filter all the first and last privote walls
            foreach (Privote privote in lstPrivote)
            {
                if (privote.position.z == minLimit.z)
                    lstStartPrivote.Add(privote.id);
            }

            int randStart = lstStartPrivote[Random.Range(0, lstStartPrivote.Count)];
            Vector3 firstpos = lstPrivote[randStart].position;
            DestroyImmediate(lstPrivote[randStart].privote);
            return firstpos;
        }

        public void SpawnBrick(Vector3 position)
        {
            GameObject privoteBrick = GameObject.Instantiate(prefabPrivoteBrick, position, Quaternion.identity);
            privoteBrick.transform.SetParent(blockTransform);
        }

        public Vector3 AutomationDirectionBrick(Vector3 beforePos, Vector3 currentPos)
        {
            List<Vector3> lstNextPos = new();
            Vector3 nextPos;
            Vector3 posLeft = currentPos + Vector3.left;
            Vector3 posRight = currentPos + Vector3.right;
            Vector3 posForward = currentPos + Vector3.forward;
            Vector3 posBack = currentPos + Vector3.back;
            lstNextPos.Add(posLeft);
            lstNextPos.Add(posRight);
            lstNextPos.Add(posForward);
            lstNextPos.Add(posBack);
            lstNextPos.RemoveAll(x => CheckPositionInLimitSpawn(x));
            do
            {
                nextPos = lstNextPos[Random.Range(0, lstNextPos.Count)];
            } while (nextPos == beforePos);
            return nextPos;
        }

        private bool CheckPositionInLimitSpawn(Vector3 posCheck)
        {
            if (posCheck.x < minLimit.x + 1f || posCheck.x > maxLimit.x - 1f || posCheck.z < minLimit.z + 1f)
                return true;
            return false;
        }

        [Button("Destroy Line Brick")]
        public void DestroyLineBrick()
        {
            foreach (Vector3 pos in lstBrickBody)
            {
                DestroyImmediate(lstPrivote.Find(x => x.position == pos).privote);
            }
            SpawnBodyBrick();
        }
        [Button("Spawn Body Brick")]

        public void SpawnBodyBrick()
        {
            foreach (Vector3 posSpawn in lstBrickBody)
            {
                SpawnBrick(posSpawn);
            }
        }

        //[MenuItem("Tools/Save Prefab")]
        [Button("Save Prefab")]
        public static void SavePrefab()
        {
            string folderPath = "Assets/Prefabs/Blocks";
            string[] prefabPaths = AssetDatabase.FindAssets("Block_", new[] { folderPath });

            GameObject selectedObject = Selection.activeGameObject;
            int numberBlock = prefabPaths.Length + 1;
            string prefabPath = "Assets/Prefabs/Blocks/Block_" + numberBlock.ToString() + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(selectedObject, prefabPath);
        }
    }
}
