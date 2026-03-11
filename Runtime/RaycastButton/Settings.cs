using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using TMPro;
using UnityEngine.UI;
using Andrey04o.Chess;

namespace Andrey04o.RaycastButton {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Settings : UdonSharpBehaviour
    {
        public bool isAutoLock = false;
        public bool isShowDesktopButton = false;
        public float sensitivity;
        public Toggle toggleAutoLock;
        public Toggle toggleShowDesktopButton;
        public TextMeshProUGUI textMeshSensitivity;
        public DesktopControls desktopControls;
        public ChessButtons chessButtons;

        public void MouseSpeedUp() {
            sensitivity++;
            textMeshSensitivity.text = sensitivity + "";
        }
        public void MouseSpeedDown() {
            sensitivity--;
            textMeshSensitivity.text = sensitivity + "";
        }
        public void AutoLock() {
            isAutoLock = toggleAutoLock.isOn;
        }
        public void ShowDesktopButton() {
            isShowDesktopButton = toggleShowDesktopButton.isOn;
            chessButtons.ShowDesktopButtons(true);
        }

    }
}
