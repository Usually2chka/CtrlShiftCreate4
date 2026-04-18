using System;
using _Bifrost.Runtime.Managers.GamePlay;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace _Bifrost.UI.Controllers
{
    public class HUDController : MonoBehaviour
    {
        public bool IsFullInventory {get; private set;}
        
        private VisualElement _root;

        private VisualElement _hotBar;
        private VisualElement[] _cells;
        private InteractiveObject[] _cellsHotbar;
        private Label _hintLabel;
        
        private int _selectedIndex;
        
        
        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _root.style.display = DisplayStyle.None;
            HideSelf();
            _hintLabel = _root.Q<Label>("HintLabel");
            _hintLabel.style.display = DisplayStyle.None;
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
        
        public bool AddToHotbar(InteractiveObject obj)
        {
            if (_cellsHotbar[_selectedIndex] == null)
            {
                _cellsHotbar[_selectedIndex] = obj;
                RefreshSlot(_selectedIndex);
                obj.gameObject.SetActive(false);
                return true;
            }

            for (int i = 0; i < _cellsHotbar.Length; i++)
            {
                if (_cellsHotbar[i] == null)
                {
                    _cellsHotbar[i] = obj;
                    RefreshSlot(i);
                    obj.gameObject.SetActive(false);
                    return true;
                }
            }
            
            IsFullInventory = true;
            return false;
        }
        public bool CanAddToHotbar()
        {
            // если хотя бы один слот пустой
            for (int i = 0; i < _cellsHotbar.Length; i++)
            {
                if (_cellsHotbar[i] == null)
                    return true;
            }

            return false;
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
            _hintLabel.style.display = DisplayStyle.None;
        }
        
        public void ShowHint(string text)
        {
            _hintLabel.style.color = Color.black;
            _hintLabel.text = text;
            _hintLabel.style.display = DisplayStyle.Flex;
        }
        
        public void ShowError(string text)
        {
            _hintLabel.text = text;
            _hintLabel.style.display = DisplayStyle.Flex;
            _hintLabel.style.color = Color.red;
        }

        public void HideSelf()
        {
            _root.style.display = DisplayStyle.None;
        }
        
        public InteractiveObject GetSelectedItem()
        {
            return _cellsHotbar[_selectedIndex];
        }
        
        public void RemoveSelectedItem()
        {
            _cellsHotbar[_selectedIndex] = null;
            RefreshSlot(_selectedIndex);
        }
    }
}