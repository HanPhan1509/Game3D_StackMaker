using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] private AudioManager audio;
        [SerializeField] private Model model;
        [SerializeField] private View view;
        [SerializeField] private Player player;
        [SerializeField] private Transform gameSpace;
        private List<Map> maps = new();
        private bool isGameover = false;
        private int levelMap = 0;

        private void Start()
        {
            audio.PlaySound(SoundType.soundBG);
            LoadMap();
            isGameover = true;
            view.UI_Start.gameObject.SetActive(true);
            player.Init(model.SpeedMoving, GameOver);
        }

        private void LoadMap()
        {
            string folderPath = "Assets/Resources";
            string[] prefabPaths = AssetDatabase.FindAssets("Map_", new[] { folderPath });
            foreach (string prefabPath in prefabPaths)
            {
                string prefabFullPath = AssetDatabase.GUIDToAssetPath(prefabPath);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFullPath);
                maps.Add(prefab.GetComponent<Map>());
            }
        }    

        void Update()
        {
            ControlInput();
        }

        private void ControlInput()
        {
            if (isGameover)
                return;

            if (Input.GetKeyDown(KeyCode.W))
                player.ChangeStatePlayer(StatePlayer.MoveForward);
            if (Input.GetKeyDown(KeyCode.S))
                player.ChangeStatePlayer(StatePlayer.MoveBackward);
            if (Input.GetKeyDown(KeyCode.D))
                player.ChangeStatePlayer(StatePlayer.MoveRight);
            if (Input.GetKeyDown(KeyCode.A))
                player.ChangeStatePlayer(StatePlayer.MoveLeft);
        }

        private void GameOver()
        {
            isGameover = true;
            view.UI_GameOver.gameObject.SetActive(true);
            view.UI_GameOver.ShowPopup(levelMap == maps.Count - 1);
        }

        public void ButtonPlay()
        {
            isGameover = false;
            GameObject mapSpace = new GameObject("Map");
            mapSpace.transform.SetParent(gameSpace);
            Debug.Log(maps[levelMap].gameObject.name);
            Map map = GameObject.Instantiate(maps[levelMap].gameObject).GetComponent<Map>();
            map.transform.SetParent(mapSpace.transform);
            player.transform.position = new Vector3(map.posPlayer.x, 3.055f, map.posPlayer.z);
            player.Reset();
        }    

        public void ButtonReplay()
        {
            SceneManager.LoadScene("Game");
        }

        public void ButtonNext()
        {
            DestroyImmediate(gameSpace.GetChild(0).gameObject);
            view.UI_GameOver.gameObject.SetActive(false);
            levelMap++;
            player.Reset();
            ButtonPlay();
        }    
    }
}