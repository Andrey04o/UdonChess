using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using TMPro;
using Andrey04o.Chess;
using UnityEngine.UI;
namespace Andrey04o.RaycastButton {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DesktopControls : UdonSharpBehaviour
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
        public Settings settings;
        public Locker lockerCurrent;
        public Image imageLock;
        public TextMeshProUGUI textLock;
        void Update()
        {
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.R)) {
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
            settings.MouseSpeedUp();
            cursorController.sensitivity = settings.sensitivity;
            textMeshMouseSpeed.text = cursorController.sensitivity + "";
        }
        public void MouseSpeedDown() {
            settings.MouseSpeedDown();
            cursorController.sensitivity = settings.sensitivity;
            textMeshMouseSpeed.text = cursorController.sensitivity + "";
        }
        public void Leave() {
            if (lockerCurrent != null)
                lockerCurrent.desktopControls = null;
            stationDesktopView.Leave();
        }
        public void ToggleInterfaceWindow() {
            interfaceWindow.SetActive(!interfaceWindow.activeSelf);
            interfaceWindowHided.SetActive(!interfaceWindow.activeSelf);
        }
        public void Show(bool value, Locker locker, bool side = true) {
            gameObject.SetActive(value);
            this.side = side;
            ShowSide();
            cursorController.sensitivity = settings.sensitivity;
            textMeshMouseSpeed.text = cursorController.sensitivity + "";
            lockerCurrent = locker;
            lockerCurrent.desktopControls = this;
            lockerCurrent.ShowCurrent(ref textLock, ref imageLock);
        }
        public void Hide() {
            gameObject.SetActive(false);
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
        public void LockerUse() {
            lockerCurrent.Use();
        }
    }
}
