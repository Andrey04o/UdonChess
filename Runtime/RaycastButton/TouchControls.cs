using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using Andrey04o.Chess;
using VRC.SDKBase;
using VRC.SDK3.Components;
namespace Andrey04o.Chess.RaycastButton {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TouchControls : UdonSharpBehaviour
    {
        public TileTouch[] tileTouches;
        public GameField gameField;
        public GameObject canvas;
        public bool isEnabledTouchSupport = true;
        bool isFirstTime = true;
        public ChessButtons chessButtons;
        public void ShowTiles(bool value) {
            foreach (TileTouch tileTouch in tileTouches) {
                tileTouch.gameObject.SetActive(value);
            }
        }
        public void ChangeMethod(bool value) {
            if (value == true) {
                ChangeMethod(VRCInputMethod.Touch);
            } else {
                ChangeMethod(VRCInputMethod.Mouse);
            }
        }

        public void ChangeMethod(VRCInputMethod inputMethod) {
            if (inputMethod == VRCInputMethod.Touch) {
                isEnabledTouchSupport = true;
                canvas.SetActive(true);
                gameField.isTouchMode = true;
                gameField.ShowPieces(Quaternion.identity);
            } else {
                isEnabledTouchSupport = false;
                canvas.SetActive(false);
                gameField.isTouchMode = false;
                gameField.ShowPieces(Quaternion.identity);
            }
            if (isEnabledTouchSupport)
                chessButtons.textMeshTouchScreen.text = "Touch screen<br><color=#1f512b>On";
            else
                chessButtons.textMeshTouchScreen.text = "Touch screen<br><color=#d00000>Off";
        }
    }
}
