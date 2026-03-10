using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VRC.Udon;
using VRC.Udon.Common;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
//using UdonSharpEditor;
using UnityEditor;
#endif
namespace Andrey04o.RaycastButton {
    public class UIButtonController : RaycastButton
    {
        
        //[SerializeField] private UnityEvent onSubmit = new UnityEvent();
        private bool isCursorHovering = false;
        //public Button button;
        public BoxCollider boxCollider;
        public RectTransform rectTransform;
        public Selectable selectable;

        //public ObjectBuying[] udonBehaviours = new ObjectBuying[0];
        public string[] udonEvents = new string[0];
        BaseEventData eventData = new BaseEventData(EventSystem.current);
        PointerEventData pointerEvent = new PointerEventData(EventSystem.current);
        public Button button;
        void Start() {
            if (boxCollider == null) {
                boxCollider = GetComponent<BoxCollider>();
            }
            if (rectTransform == null) {
                rectTransform = GetComponent<RectTransform>();
            }
            if (selectable == null) {
                selectable = GetComponent<Selectable>();
            }
        }
        #if !COMPILER_UDONSHARP && UNITY_EDITOR
        public void CreateCollider() {
            if (boxCollider == null) {
                boxCollider = gameObject.AddComponent<BoxCollider>();
                Vector3 size = rectTransform.sizeDelta;
                size.z = 9f;
                boxCollider.size = size;
                
                // Adjust center based on RectTransform pivot
                Vector2 pivot = rectTransform.pivot;
                Vector2 sizeDelta = rectTransform.sizeDelta;
                
                // Calculate offset from center based on pivot
                // Pivot (0,0) is bottom-left, (0.5,0.5) is center, (1,1) is top-right
                float offsetX = (0.5f- pivot.x ) * sizeDelta.x;
                float offsetY = (0.5f- pivot.y ) * sizeDelta.y;
                
                boxCollider.center = new Vector3(offsetX, offsetY, 0f);
            }
        }
        #endif

        override public void OnRaycastEnter(bool isUnityMouse = false)
        {
            // Check if the colliding object is from CursorController
                isCursorHovering = true;
                selectable.OnPointerEnter(pointerEvent);
                Debug.Log("pointer enter");
        }

        public override void InputUse(bool value, UdonInputEventArgs args)
        {
            /*
            if (value == true) {
                Press();
            }
            */
        }

        public void Press() {
            if (isCursorHovering) {
                // Update pointer event position from cursor controller
                
                //if (selectable!= null) selectable.OnSelect(pointerEvent);
                if (button!= null) button.OnSubmit(eventData);
                
                Debug.Log("Press omg");
            }
        }
        
        /*
        private void OnTriggerStay(Collider other)
        {
            // Check if cursor is still hovering and left mouse button is pressed
            if (isCursorHovering && CheckIsCursor(other))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //Debug.Log("Button pressed: " + gameObject.name);

                }
            }
        }
        */

        public override void OnRaycastExit(bool isUnityMouse = false)
        {
            // Check if the cursor left this button
                isCursorHovering = false;
                //Debug.Log("Cursor exited button: " + gameObject.name);
                selectable.OnPointerExit(pointerEvent);
        }

        public override void OnRaycastClick() {
            Press();
        }

        private bool CheckIsCursor(Collider other) {
            return other.GetComponent<VirtualCursor>() != null;
        }
        
    }
}