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
        private IInventoryItem[] _cellsHotbar;
        private Label _hintLabel;
        
        private Button _tutorialButton;
        private VisualElement _tutorialOverlay;
        private Image _tutorialImage;
        private Button _closeTutorialButton;
        private Image _tutorialIcon;
        
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
            _cellsHotbar = new IInventoryItem[3];
            
            _tutorialButton = _root.Q<Button>("TutorialButton");
            _tutorialOverlay = _root.Q<VisualElement>("TutorialOverlay");
            _tutorialImage = _root.Q<Image>("TutorialImage");
            _closeTutorialButton = _root.Q<Button>("CloseTutorialButton");
            _tutorialIcon = _root.Q<Image>("TutorialIcon");
            
            // Load tutorial image
            Texture2D tutorialTexture = Resources.Load<Texture2D>("tutorial");
            if (tutorialTexture != null)
            {
                _tutorialImage.image = tutorialTexture;
            }
            
            // Load tutorial icon
            Texture2D iconTexture = Resources.Load<Texture2D>("tutorial_icon");
            if (iconTexture != null)
            {
                _tutorialIcon.image = iconTexture;
            }
            
            _tutorialButton.clicked += OnTutorialButtonClicked;
            _closeTutorialButton.clicked += OnCloseTutorialClicked;
            
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
            
            if (Keyboard.current[Key.H].wasPressedThisFrame) ToggleTutorial();
            if (Keyboard.current[Key.Escape].wasPressedThisFrame)
            {
                if (IsTutorialOpen)
                    HideTutorial();
            }
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
        
        public bool AddToHotbar(IInventoryItem obj)
        {
            if (_cellsHotbar[_selectedIndex] == null)
            {
                _cellsHotbar[_selectedIndex] = obj;
                RefreshSlot(_selectedIndex);
                var crystal = obj as Crystal;
                if (crystal != null)
                {
                    crystal.Hide();
                }
                else
                {
                    (obj as InteractiveObject).gameObject.SetActive(false);
                }
                return true;
            }

            for (int i = 0; i < _cellsHotbar.Length; i++)
            {
                if (_cellsHotbar[i] == null)
                {
                    _cellsHotbar[i] = obj;
                    RefreshSlot(i);
                    var crystal = obj as Crystal;
                    if (crystal != null)
                    {
                        crystal.Hide();
                    }
                    else
                    {
                        (obj as InteractiveObject).gameObject.SetActive(false);
                    }
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
        
        public IInventoryItem GetSelectedItem()
        {
            return _cellsHotbar[_selectedIndex];
        }
        
        public void RemoveSelectedItem()
        {
            _cellsHotbar[_selectedIndex] = null;
            RefreshSlot(_selectedIndex);
        }
        
        public void RefreshAllSlots()
        {
            for (int i = 0; i < _cells.Length; i++)
            {
                RefreshSlot(i);
            }
        }
        
        private void OnTutorialButtonClicked() { ShowTutorial(); }
        
        private void OnCloseTutorialClicked() { HideTutorial(); }
        
        private void ShowTutorial() 
        { 
            _tutorialOverlay.style.display = DisplayStyle.Flex; 
            GameManager.Instance.Player.DisableInputForUI();
        }
        
        private void HideTutorial() 
        { 
            _tutorialOverlay.style.display = DisplayStyle.None; 
            GameManager.Instance.Player.EnableInputForUI();
        }
        
        private void ToggleTutorial() 
        { 
            if (_tutorialOverlay.style.display == DisplayStyle.Flex) 
                HideTutorial(); 
            else 
                ShowTutorial(); 
        }
        
        public bool IsTutorialOpen => _tutorialOverlay.style.display == DisplayStyle.Flex;
    }
}