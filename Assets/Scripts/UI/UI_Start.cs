using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class UI_Start : MonoBehaviour
    {
        [SerializeField] private UnityEvent OnButtonPlay;

        public void ButtonPlay()
        {
            OnButtonPlay?.Invoke();
            this.gameObject.SetActive(false);
        }
    }
}
