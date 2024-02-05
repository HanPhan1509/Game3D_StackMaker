using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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

        private List<Privote> lstPrivote = new();
        private float widthBridge = 0.0f;
        private float widthPrivote = 0.0f;
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
            int id = 0;
            for (float x = minLimit.x; x < maxLimit.x; x += widthPrivote)
            {
                for (float z = minLimit.z; z < maxLimit.z; z += widthPrivote)
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
        }
        [Button("Line Brick")]
        
        public void LineBrick()
        {
            List<int> tempPrivote = new();
            foreach(Privote privote in lstPrivote)
            {
                if (privote.position.z == minLimit.z)
                    tempPrivote.Add(privote.id);
            }
            int rand = tempPrivote[Random.Range(0, tempPrivote.Count)];
            GameObject privoteBrick = SimplePool.Spawn(prefabPrivoteBrick, lstPrivote[rand].position, Quaternion.identity);
            privoteBrick.transform.SetParent(map);
            SimplePool.Despawn(lstPrivote[rand].privote);
        }    
    }
}
