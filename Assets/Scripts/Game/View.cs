using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class View : MonoBehaviour
    {
        [SerializeField] private UI_GameOver ui_GameOver;
        [SerializeField] private UI_Start ui_Start;

        public UI_GameOver UI_GameOver => ui_GameOver;
        public UI_Start UI_Start => ui_Start;
    }
}
