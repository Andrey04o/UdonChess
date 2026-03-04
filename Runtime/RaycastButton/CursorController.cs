using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
//using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using VRC.Udon.Common;
using UdonSharp;
/// <summary>
/// In-game UI cursor that lives on a Canvas. Provides methods to set sprite and move the cursor.
/// - Creates an Image child under the assigned `targetCanvas` if `cursorImage` isn't assigned
/// - Methods available: SetSprite, MoveToScreenPosition, MoveByScreenDelta, MoveToWorldPosition
/// - Cursor position is clamped so it won't move outside the canvas/screen
/// </summary>
namespace Andrey04o.RaycastButton {
    [DisallowMultipleComponent] [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class CursorController : UdonSharpBehaviour
    {
        [Header("References")]
        [Tooltip("Canvas used for the cursor. If null, will try to find a Canvas in the scene.")]
        public Canvas targetCanvas;

        [Tooltip("Optional Image to use for the cursor. If null, one will be created at runtime under the target Canvas.")]
        public Image cursorImage;

        [Header("Cursor Settings")]
        public float sensitivity = 2f;
        [Tooltip("Default sprite for the cursor (optional)")]
        public Sprite defaultSprite;

        [Tooltip("Size in pixels for the cursor RectTransform. Set 0 to use native sprite size if available.")]
        public Vector2 size = new Vector2(32, 32);

        [Tooltip("If true the cursor will be clamped inside the canvas rect/screen")]
        public bool clampToCanvas = true;

        [Tooltip("Offset in pixels applied after positioning (screen-space) — useful to shift hotspot")]
        public Vector2 hotspotOffset = Vector2.zero;

        public RectTransform _canvasRect;
        private RectTransform _cursorRect;
        [HideInInspector] public Vector2 currentPosition;
        [HideInInspector] public Vector2 currentPositionScreen;
        [HideInInspector] public Vector2 velocity = Vector2.zero;
        [HideInInspector] public Vector3 positionReal;
        public bool isLocked = false;
        //public InputSystemUIInputModule asd;
        // Expose canvas rect for other systems that need canvas dimensions

        void Start()
        {
            _canvasRect = targetCanvas.GetComponent<RectTransform>();

            _cursorRect = cursorImage.GetComponent<RectTransform>();

            if (defaultSprite != null)
                SetSprite(defaultSprite);
        }
        /// <summary>
        /// Set the sprite used by the cursor. If size is zero, will optionally set native size.
        /// </summary>
        void Update() {
            velocity.x = Input.GetAxis("Mouse X");
            velocity.y = Input.GetAxis("Mouse Y");
            Move(velocity);
        }
        public void SetSprite(Sprite spr, bool useNativeSize = false)
        {
            cursorImage.sprite = spr;
            if (useNativeSize && spr != null)
            {
                _cursorRect.sizeDelta = new Vector2(spr.rect.width, spr.rect.height);
            }
            else if (size != Vector2.zero)
            {
                _cursorRect.sizeDelta = size;
            }
        }
        
        public void Move(Vector2 delta) {
            if (isLocked == true) return;
            _cursorRect.anchoredPosition = _cursorRect.anchoredPosition + delta * sensitivity;
            ClampCursorToCanvas();
            currentPosition = _cursorRect.anchoredPosition;
            currentPositionScreen = currentPosition + _canvasRect.rect.size * 0.5f;
        }

        /// <summary>
        /// Set cursor position using screen-space coordinates (pixels, origin bottom-left).
        /// This converts the screen position into the canvas anchoredPosition using the canvas rect
        /// and then clamps the cursor inside the canvas.
        /// </summary>
        public void SetPositionFromScreen(Vector2 screenPosition)
        {
            //Vector2 canvasSize = _canvasRect.rect.size;
            //Vector2 halfCanvas = canvasSize * 0.5f;

            //Vector2 anchored = screenPosition - halfCanvas + hotspotOffset;

            //_cursorRect.anchoredPosition = anchored;
            _cursorRect.anchoredPosition = screenPosition;
            ClampCursorToCanvas();
            currentPosition = _cursorRect.anchoredPosition;
            currentPositionScreen = currentPosition + _canvasRect.rect.size * 0.5f;
        }

        /// <summary>
        /// Clamp the cursor RectTransform so it stays inside the canvas rect.
        /// Works for Screen Space - Overlay and Screen Space - Camera canvases.
        /// </summary>
        public void ClampCursorToCanvas()
        {
            Vector2 canvasSize = _canvasRect.rect.size;
            Vector2 halfCanvas = canvasSize * 0.5f;
            //Vector2 cursorSize = _cursorRect.rect.size;

            // anchoredPosition is relative to the pivot; treat pivot as center of canvas
            Vector2 min = -halfCanvas;// + (cursorSize * _cursorRect.pivot);
            Vector2 max = halfCanvas;// - (cursorSize * (Vector2.one - _cursorRect.pivot));

            Vector2 anchored = _cursorRect.anchoredPosition;
            anchored.x = Mathf.Clamp(anchored.x, min.x, max.x);
            anchored.y = Mathf.Clamp(anchored.y, min.y, max.y);

            _cursorRect.anchoredPosition = anchored;
        }
        public override void InputLookVertical(float value, UdonInputEventArgs args)
        {
            base.InputLookVertical(value, args);
            //velocity.y = value;
            
        }

        public override void InputLookHorizontal(float value, UdonInputEventArgs args)
        {
            base.InputLookHorizontal(value, args);
            //velocity.x = value;
        }
        public void SetVisibility(bool value) {
            cursorImage.enabled = value;
        }
    }
}
