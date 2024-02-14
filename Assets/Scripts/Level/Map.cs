using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game
{
    [CreateAssetMenu(fileName = "Map", menuName = "Maps/New Map", order = 1)]
    public class Map : MonoBehaviour
    {
        public int ID;
        public Block startBlock;
        public List<Block> blocks;
        public Block endBlock;
    }
}
