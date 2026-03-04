using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace Andrey04o.Chess {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class CellTrigger : UdonSharpBehaviour
    {
        public RaycastButton.Cell cell;
        void OnTriggerEnter(Collider collider) {
            PiecePickup piecePickup = collider.GetComponent<PiecePickup>();
            if (piecePickup != null) {
                piecePickup.AddCell(cell);
            }
        }
        void OnTriggerExit(Collider collider) {
            PiecePickup piecePickup = collider.GetComponent<PiecePickup>();
            if (piecePickup != null) {
                piecePickup.RemoveCell();
            }
        }
    }
}

