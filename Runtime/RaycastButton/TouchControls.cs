using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using Andrey04o.Chess;
using VRC.SDKBase;
using VRC.SDK3.Components;
namespace Andrey04o.RaycastButton {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TouchControls : UdonSharpBehaviour
    {
        public TileTouch[] tileTouches;
        public GameField gameField;
        public GameObject canvas;
        public bool isEnabledTouchSupport = true;
        bool isFirstTime = true;
        public void ShowTiles(bool value) {
            foreach (TileTouch tileTouch in tileTouches) {
                tileTouch.gameObject.SetActive(value);
            }
        }
        void Start() {
            //ChangeMethod(VRC.SDKBase.InputManager.GetLastUsedInputMethod());
            ChangeMethod(VRCInputMethod.Touch);
            //Debug.Log(VRC.SDKBase.InputManager.GetLastUsedInputMethod());
        }

        public override void OnInputMethodChanged(VRCInputMethod inputMethod)
        {
            //ChangeMethod(inputMethod);
        }

        public void ChangeMethod(VRCInputMethod inputMethod) {
            if (inputMethod == VRCInputMethod.Touch) {
                    isEnabledTouchSupport = true;
                    canvas.SetActive(true);
                    foreach(Piece piece in gameField.pieces.InTableAll) {
                        piece.meshCollider.enabled = false;
                    }
            } else {
                    isEnabledTouchSupport = false;
                    canvas.SetActive(false);
                    foreach(Piece piece in gameField.pieces.InTableAll) {
                        piece.meshCollider.enabled = true;
                    }
                
            }
        }
    }
}
