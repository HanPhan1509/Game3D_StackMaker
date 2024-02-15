using System.Collections.Generic;
using UnityEngine;


namespace Game
{
    [CreateAssetMenu(fileName = "Map", menuName = "Maps/New Map", order = 1)]
    public class Map : MonoBehaviour
    {
        public int ID;
        public Vector3 posPlayer;
        public int quantityBlock;
        public Block startBlock;
        public List<Block> blocks;
        public Block endBlock;
    }
}
