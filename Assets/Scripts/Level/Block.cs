using NaughtyAttributes;
using System.Collections.Generic;
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

        private Vector3 minLimit;
        private Vector3 maxLimit;

        private float minQuantityPrivote;
        private float maxQuantityPrivote;
        private float widthPrivote;

        private List<Privote> lstPrivoteDespawn = new();
        private List<Privote> lstPrivote = new();

        public Vector3 FirstBrick { get => firstBrick; set => firstBrick = value; }
        public Vector3 LastBrick1 { get => lastBrick; set => lastBrick = value; }
        public List<Vector3> LstBrickBody { get => lstBrickBody; set => lstBrickBody = value; }

        public void Init(Vector3 minLimit, Vector3 maxLimit, float minQuantityPrivote, float maxQuantityPrivote, float widthPrivote)
        {
            this.minLimit = minLimit;
            this.maxLimit = maxLimit;
            this.minQuantityPrivote = minQuantityPrivote;
            this.maxQuantityPrivote = maxQuantityPrivote;
            this.widthPrivote = widthPrivote;
        }

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
            Vector3 firstBrick = SpawnFirstPrivoteBrick();
            SpawnBrick(firstBrick);

            do
            {
                lstBrickBody.Clear();
                Vector3 nextPos = firstBrick;
                Vector3 currentPos = firstBrick;
                Vector3 beforePos = firstBrick;
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

            LastBrick();
            DestroyLineBrick();
        }

        private void LastBrick()
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

        private (bool, int) CheckDuplicatePosition(Vector3 pos, int count)
        {
            bool isDuplicated = false;
            if (count == 0)
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
            GameObject privoteBrick = GameObject.Instantiate(prefabPrivoteBrick, position, Quaternion.identity);
            privoteBrick.transform.SetParent(blockTransform);
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
            GameObject selectedObject = Selection.activeGameObject;
            string prefabPath = "Assets/Prefabs/Blocks/Block_" + selectedObject.name + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(selectedObject, prefabPath);

            //Debug.Log("Prefab đã được lưu tại đường dẫn: " + prefabPath);
        }
    }
}
