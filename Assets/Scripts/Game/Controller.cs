using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] private Model model;
        [SerializeField] private View view;
        [SerializeField] private Player player;

        private void Start()
        {
            //Time.timeScale = 0.1f;
            player.Init(model.SpeedMoving);
        }

        void Update()
        {
            ControlInput();
        }

        private void ControlInput()
        {
            if (Input.GetKeyDown(KeyCode.W))
                player.ChangeStatePlayer(StatePlayer.MoveForward);
            if (Input.GetKeyDown(KeyCode.S))
                player.ChangeStatePlayer(StatePlayer.MoveBackward);
            if (Input.GetKeyDown(KeyCode.D))
                player.ChangeStatePlayer(StatePlayer.MoveRight);
            if (Input.GetKeyDown(KeyCode.A))
                player.ChangeStatePlayer(StatePlayer.MoveLeft);
        }
    }
}