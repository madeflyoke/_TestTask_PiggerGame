using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pigger.GUI
{
    [RequireComponent(typeof(Button))]
    public abstract class BaseButton : MonoBehaviour
    {
        protected Button button;

        protected virtual void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(Listeners);
        }
        private void OnDisable()
        {
            button.onClick.RemoveListener(Listeners);
        }

        protected virtual void Listeners()
        {

        }
    }
}

