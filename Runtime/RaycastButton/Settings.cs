using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using TMPro;

namespace Andrey04o.RaycastButton {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Settings : UdonSharpBehaviour
    {
        public StationDesktopView stationDesktopView;
        public CursorController cursorController;
        public GameObject interfaceWindow;
        public TextMeshProUGUI textMeshMouseSpeed;
        public bool side;
        bool currentTurn = false;
        public Transform arrowTurn;
        public TextMeshProUGUI textMeshArrow;
        public GameObject interfaceWindowHided;
        public Quaternion lookUp;
        public Quaternion lookDown;
        public GameObject winnerWindow;
        public TextMeshProUGUI textMeshWinner;
        void Update()
        {
            if (Input.GetMouseButtonDown(1)) {
                Leave();
            }
            if (Input.GetKeyDown(KeyCode.I)) {
                MouseSpeedDown();
            }
            if (Input.GetKeyDown(KeyCode.P)) {
                MouseSpeedUp();
            }
            if (Input.GetKeyDown(KeyCode.H)) {
                ToggleInterfaceWindow();
            }
        }

        public void MouseSpeedUp() {
            cursorController.sensitivity++;
            textMeshMouseSpeed.text = cursorController.sensitivity + "";
        }
        public void MouseSpeedDown() {
            cursorController.sensitivity--;
            textMeshMouseSpeed.text = cursorController.sensitivity + "";
        }
        public void Leave() {
            stationDesktopView.Leave();
        }
        public void ToggleInterfaceWindow() {
            interfaceWindow.SetActive(!interfaceWindow.activeSelf);
            interfaceWindowHided.SetActive(!interfaceWindow.activeSelf);
        }
        public void Show(bool value, bool side = true) {
            gameObject.SetActive(value);
            this.side = side;
            ShowSide();
        }
        public void ChandeSide(bool side) {
            currentTurn = side;
            ShowSide();
        }
        void ShowSide() {
            if (currentTurn == side) {
                textMeshArrow.text = "Your turn";
                arrowTurn.localRotation = lookDown;
            } else {
                arrowTurn.localRotation = lookUp;
                if (side == true) {
                    textMeshArrow.text = "White turn";
                } else {
                    textMeshArrow.text = "Black turn";
                }
            }
        }
    }
}
