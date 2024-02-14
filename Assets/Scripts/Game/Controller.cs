using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] private Model model;
        [SerializeField] private View view;
        [SerializeField] private Player player;
        private bool isGameover = false;

        private void Start()
        {
            isGameover = true;
            view.UI_Start.gameObject.SetActive(true);
            player.Init(model.SpeedMoving, GameOver);
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
            view.UI_GameOver.ShowPopup();
        }

        public void ButtonPlay()
        {
            isGameover = false;
        }    

        public void ButtonReplay()
        {
            SceneManager.LoadScene("Game");
        }

        public void ButtonNext()
        {
            Debug.Log("Next");
        }    
    }
}