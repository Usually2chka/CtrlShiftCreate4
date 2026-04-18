using System;
using _Bifrost.Runtime.Managers.GamePlay;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace _Bifrost.UI.Controllers
{
    public class HUDController : MonoBehaviour
    {
        private VisualElement _root;

        private VisualElement _hotBar;
        private VisualElement[] _cells;
        private InteractiveObject[] _cellsHotbar;
        
        private int _selectedIndex;

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _root.style.display = DisplayStyle.None;
            
            _hotBar = _root.Q<VisualElement>("HotBar");
            _cells = new VisualElement[3];
            _cellsHotbar = new InteractiveObject[3];
            
            for (int i = 0; i < _cells.Length; i++)
                _cells[i] = _root.Q<VisualElement>($"cell{i}");
            
            UpdateSelection();
        }
        
        void UpdateSelection()
        {
            for (int i = 0; i < _cells.Length; i++)
            {
                _cells[i].RemoveFromClassList("selected");
            }

            _cells[_selectedIndex].AddToClassList("selected");
        }        
        
        private void Update()
        {
            if (Keyboard.current[Key.Digit1].wasPressedThisFrame) Select(0);
            if (Keyboard.current[Key.Digit2].wasPressedThisFrame) Select(1);
            if (Keyboard.current[Key.Digit3].wasPressedThisFrame) Select(2);
        }
        
        private void Select(int index)
        {
            _selectedIndex = index;
            UpdateSelection();
        }
        
        public void Show()
        {
            _root.style.display = DisplayStyle.Flex;
        }
        
        public void AddToHotbar(InteractiveObject obj)
        {
            for (int i = 0; i < _cellsHotbar.Length; i++)
            {
                if (_cellsHotbar[i] == null)
                {
                    _cellsHotbar[i] = obj;

                    //obj.gameObject.SetActive(false); // спрятать в мире
                    RefreshSlot(i);

                    return;
                }
            }
        }
        
        private void RefreshSlot(int index)
        {
            if (_cellsHotbar[index] != null)
            {
                _cells[index].style.backgroundImage =
                    new StyleBackground(_cellsHotbar[index].Icon);
            }
            else
            {
                _cells[index].style.backgroundImage = null;
            }
        }

        public void HideHint()
        {
            //hintLabel.style.display = DisplayStyle.None;
        }
        
        public void ShowHint(string text)
        {
            // hintLabel.text = text;
            // hintLabel.style.display = DisplayStyle.Flex;
        }
    }
}