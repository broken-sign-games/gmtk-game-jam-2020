using GMTK2020.Data;
using GMTK2020.Input;
using GMTK2020.TutorialSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GMTK2020.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class ToolPanel : MonoBehaviour
    {
        [SerializeField] private float deceleration = 5f;
        [SerializeField] private Camera mainCamera = null;

        private Dictionary<Tool, ToolButton> toolToButton;
        private Dictionary<Tool, int> toolToIndex;
        private List<ToolButton> toolButtons;

        private InputActions inputs;
        private bool isDragging = false;
        private float speed = 0f;
        private Vector2 lastPointerPos;

        private float minXPos;

        private RectTransform rectTransform;

        private TutorialManager tutorialManager;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            toolToButton = new Dictionary<Tool, ToolButton>();
            toolToIndex = new Dictionary<Tool, int>();
            toolButtons = new List<ToolButton>();

            foreach (Transform child in transform)
            {
                var toolButton = child.GetComponent<ToolButton>();
                if (toolButton && toolButton.gameObject.activeSelf)
                {
                    toolToButton[toolButton.Tool] = toolButton;
                    toolToIndex[toolButton.Tool] = toolButtons.Count;
                    toolButtons.Add(toolButton);
                }
            }

            inputs = new InputActions();

            inputs.Gameplay.Select.performed += OnSelect;
            inputs.Gameplay.Select.canceled += OnRelease;

            tutorialManager = TutorialManager.Instance;
            tutorialManager.TutorialReady += OnTutorialReady;
        }

        private void Start()
        {
            var parent = rectTransform.parent as RectTransform;
            minXPos = parent.rect.width - rectTransform.rect.width;
        }

        private void OnEnable()
        {
            inputs.Enable();
        }

        private void OnDisable()
        {
            inputs.Disable();
        }

        private void OnDestroy()
        {
            inputs.Gameplay.Select.performed -= OnSelect;
            inputs.Gameplay.Select.canceled -= OnRelease;

            tutorialManager.TutorialReady -= OnTutorialReady;
        }

        private void Update()
        {
            if (isDragging)
                OnDrag();

            MovePanel();
        }

        private void MovePanel()
        {
            float newXPos = Mathf.Clamp(rectTransform.anchoredPosition.x + speed * Time.deltaTime, minXPos, 0);
            rectTransform.anchoredPosition = new Vector2(newXPos, rectTransform.anchoredPosition.y);

            if (Mathf.Abs(speed) - deceleration * Time.deltaTime < 0)
                speed = 0;
            else
                speed -= Mathf.Sign(speed) * deceleration * Time.deltaTime;
        }

        private void OnSelect(InputAction.CallbackContext ctx)
        {
            Vector2 pointerPos = inputs.Gameplay.Point.ReadValue<Vector2>();

            if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, pointerPos, mainCamera))
                return;

            isDragging = true;
            lastPointerPos = pointerPos;
        }

        private void OnDrag()
        {
            Vector2 pointerPos = inputs.Gameplay.Point.ReadValue<Vector2>();

            float deltaX = pointerPos.x - lastPointerPos.x;
            speed = deltaX / Time.deltaTime;

            lastPointerPos = pointerPos;
        }

        private void OnRelease(InputAction.CallbackContext ctx)
        {
            isDragging = false;
        }

        public Vector2[] GetButtonCornersInWorldSpace(Tool tool)
        {
            var buttonTransform = toolToButton[tool].GetComponent<RectTransform>();

            Vector3[] corners = new Vector3[4];
            buttonTransform.GetWorldCorners(corners);
            
            return corners
                .Select(corner => (Vector2)corner)
                .ToArray();
        }

        private Task OnTutorialReady(Tutorial tutorial)
        {
            if (tutorial.InteractableTools.Count == 0)
                return Task.CompletedTask;

            isDragging = false;

            int targetToolIndex = tutorial.InteractableTools
                .Max(tool => toolToIndex[tool]);

            float targetXPos = targetToolIndex / (toolToIndex.Count - 1f) * minXPos;
            float deltaXPos = targetXPos - rectTransform.anchoredPosition.x;

            speed = Mathf.Sign(deltaXPos) * Mathf.Sqrt(2 * deceleration * Mathf.Abs(deltaXPos));

            return Task.CompletedTask;
        }
    } 
}
