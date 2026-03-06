using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using TMPro;
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
        Quaternion quaternion;
        Vector3 rotation;
        public void ShowTurn(bool isBlack) {
            quaternion = arrowTurnBlack.transform.rotation;
                rotation = quaternion.eulerAngles;
            if (isBlack) {
                rotation.z = 90;
            } else {
                rotation.z = 90;
            }
            quaternion.eulerAngles = rotation;
            arrowTurnBlack.transform.rotation = quaternion;
            arrowTurnWhite.transform.rotation = quaternion;

            if (isBlack) {
                textMeshArrowBlack.text = "Your turn";
                textMeshArrowWhite.text = "Black turn";
            } else {
                textMeshArrowBlack.text = "White turn";
                textMeshArrowWhite.text = "Your turn";
            }
        }

        public void ShowWinnerWindow(byte stalemate, bool isBlack = true) {
            switch (stalemate)
            {
                case 0:
                    winnerWindow.SetActive(false);
                    return;
                case 1:
                    winnerWindow.SetActive(true);
                    textMeshWinner.text = "Stalemate";
                    return;
                case 2:
                    winnerWindow.SetActive(true);
                    if (isBlack) textMeshWinner.text = "Black won";
                    else textMeshWinner.text = "White won";
                    return;
                default:
                    return;
            }
        }
    }
}