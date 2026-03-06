using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;

namespace Andrey04o.RaycastButton {
    public class Settings : UdonSharpBehaviour
    {

        public InteractiveButtonChangeCamera interactiveButtonChangeCamera;
        public CursorController cursorController;
        void Update()
        {
            if (Input.GetMouseButtonDown(1)) {
                interactiveButtonChangeCamera.Leave();
            }
            if (Input.GetMouseButtonDown(1)) {
                interactiveButtonChangeCamera.Leave();
            }
        }
    }
}
