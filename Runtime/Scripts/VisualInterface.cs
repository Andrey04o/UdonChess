using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using TMPro;
using Andrey04o.RaycastButton;
namespace Andrey04o.Chess {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]

    public class VisualInterface : UdonSharpBehaviour
    {
        public GameObject winnerWindow;
        public TextMeshProUGUI textMeshWinner;
        public GameObject arrowTurnBlack;
        public GameObject arrowTurnWhite;
        public TextMeshProUGUI textMeshArrowBlack;
        public TextMeshProUGUI textMeshArrowWhite;
        public Quaternion quaternionUp;
        public Quaternion quaternionDown;
        Vector3 rotation;
        public DesktopControls desktopControls;
        public void ShowTurn(bool isBlack) {
            if (isBlack) {
                arrowTurnBlack.transform.rotation = quaternionDown;
                arrowTurnWhite.transform.rotation = quaternionDown;
            } else {
                arrowTurnBlack.transform.rotation = quaternionUp;
                arrowTurnWhite.transform.rotation = quaternionUp;
            }

            if (isBlack) {
                textMeshArrowBlack.text = "Your turn";
                textMeshArrowWhite.text = "Black turn";
            } else {
                textMeshArrowBlack.text = "White turn";
                textMeshArrowWhite.text = "Your turn";
            }
            desktopControls.ChandeSide(isBlack);
        }

        public void ShowWinnerWindow(byte stalemate, bool isBlack = true) {
            switch (stalemate)
            {
                case 0:
                    winnerWindow.SetActive(false);
                    desktopControls.winnerWindow.SetActive(false);
                    return;
                case 1:
                    winnerWindow.SetActive(true);
                    desktopControls.winnerWindow.SetActive(true);
                    textMeshWinner.text = "Stalemate";
                    desktopControls.textMeshWinner.text = "Stalemate";
                    return;
                case 2:
                    winnerWindow.SetActive(true);
                    desktopControls.winnerWindow.SetActive(true);
                    if (isBlack) {
                        textMeshWinner.text = "Black won";
                        desktopControls.textMeshWinner.text = "Black won";
                    }
                    else {
                        textMeshWinner.text = "White won";
                        desktopControls.textMeshWinner.text = "White won";
                    }
                    return;
                default:
                    return;
            }
        }
    }
}