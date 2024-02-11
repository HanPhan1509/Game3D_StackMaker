using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using Random = UnityEngine.Random;

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
        [SerializeField] private Transform map;
        [SerializeField] private GameObject prefabBridge;
        [SerializeField] private int quantityBrick = 10;

        [Space(2f)]
        [Header("PRIVOTE")]
        [SerializeField] private GameObject prefabPrivoteWall;
        [SerializeField] private GameObject prefabPrivoteBrick;
        [SerializeField] private Vector3 minLimit;
        [SerializeField] private Vector3 maxLimit;
        [SerializeField] private float minQuantityPrivote;
        [SerializeField] private float maxQuantityPrivote;

        [Space(2f)]
        [Header("LIST")]
        [SerializeField] private List<Vector3> lstBrickBody = new();
        [SerializeField] private List<Privote> lstPrivoteDespawn = new();
        [SerializeField] private List<Privote> lstPrivote = new();
        private float widthBridge = 0.0f;
        private float widthPrivote = 0.0f;

        [Button("Get value")]
        public void GetValue()
        {
            SimplePool.Preload(prefabPrivoteWall, 200);
            widthBridge = prefabBridge.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
            widthPrivote = prefabPrivoteWall.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.size.y;
            minQuantityPrivote = Math.Abs(maxLimit.x - minLimit.x);
            maxQuantityPrivote = minQuantityPrivote * 2;
        }

        [Button("Clear Map")]
        public void ClearMap()
        {
            lstPrivote.Clear();
            lstBrickBody.Clear();
            lstPrivoteDespawn.Clear();
            for (int i = map.childCount - 1; i >= 0; i--)
            {
                //SimplePool.Despawn(map.GetChild(i).gameObject);
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
            int id = 0;
            for (float x = minLimit.x; x <= maxLimit.x; x += widthPrivote)
            {
                for (float z = minLimit.z; z <= maxLimit.z; z += widthPrivote)
                {
                    Vector3 pos = new Vector3(x, 0, z);
                    GameObject privoteWall = SimplePool.Spawn(prefabPrivoteWall, pos, Quaternion.identity);
                    privoteWall.transform.SetParent(map);
                    Privote privote = new Privote();
                    privote.privote = privoteWall;
                    privote.position = pos;
                    privote.id = id;
                    lstPrivote.Add(privote);
                    id++;
                }
            }

            LineBrick();
        }
        private void LineBrick()
        {
            List<int> lstStartPrivote = new();
            List<int> lstEndPrivote = new();
            foreach (Privote privote in lstPrivote)
            {
                if (privote.position.z == minLimit.z)
                    lstStartPrivote.Add(privote.id);
                if (privote.position.z == maxLimit.z)
                    lstEndPrivote.Add(privote.id);
            }

            int randStart = lstStartPrivote[Random.Range(0, lstStartPrivote.Count)];
            SimplePool.Despawn(lstPrivote[randStart].privote);
            SpawnBrick(lstPrivote[randStart].position);

            do
            {
                lstBrickBody.Clear();
                Vector3 nextPos = lstPrivote[randStart].position;
                Vector3 currentPos = lstPrivote[randStart].position;
                Vector3 beforePos = lstPrivote[randStart].position;
                int count = 0;
                int indexDuplicate = 0;
                while (nextPos.z != maxLimit.z)
                {
                    bool isSpawn = true;
                    nextPos = AutomationBrick(beforePos, currentPos);
                    var check = CheckDuplicatePosition(nextPos, count);
                    isSpawn = check.Item1;
                    count = check.Item2;
                    if (isSpawn)
                    {
                        lstPrivoteDespawn.Add(lstPrivote.Find(x => x.position == nextPos));
                        lstBrickBody.Add(nextPos);
                        beforePos = currentPos;
                        currentPos = nextPos;
                    }
                    else
                    {
                        indexDuplicate = lstBrickBody.Count - 1;
                    }
                }

                if (indexDuplicate != 0)
                    lstBrickBody.RemoveAt(indexDuplicate);
            } while (lstBrickBody.Count > maxQuantityPrivote || lstBrickBody.Count < minQuantityPrivote);

            //foreach(Privote item in lstPrivoteDespawn)
            //{
            //    SimplePool.Despawn(lstPrivote.Find(x => x == item).privote);
            //}    

            foreach (Vector3 posSpawn in lstBrickBody)
            {
                SpawnBrick(posSpawn);
            }
        }

        private (bool, int) CheckDuplicatePosition(Vector3 pos, int count)
        {
            bool isDuplicated = false;
            if(count == 0)
            {
                foreach (var p in lstBrickBody)
                {
                    if (p == pos)
                    {
                        isDuplicated = true;
                        count++;
                    }
                }
            }    
            if (isDuplicated && count > 0)
                return (false, count);
            return (true, count);
        }

        public void SpawnBrick(Vector3 position)
        {
            GameObject privoteBrick = SimplePool.Spawn(prefabPrivoteBrick, position, Quaternion.identity);
            privoteBrick.transform.SetParent(map);
        }

        public Vector3 AutomationBrick(Vector3 beforePos, Vector3 currentPos)
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
    }
}
