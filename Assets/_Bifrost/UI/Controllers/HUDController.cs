using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Bifrost.UI.Controllers
{
    public class HUDController : MonoBehaviour
    {
        private VisualElement _root;

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _root.style.display = DisplayStyle.None;
        }

        public void Show()
        {
            _root.style.display = DisplayStyle.Flex;
        }
    }
}