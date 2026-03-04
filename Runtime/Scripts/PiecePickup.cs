using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace Andrey04o.Chess {
    public class PiecePickup : UdonSharpBehaviour
    {
        public Piece piece;
        public RaycastButton.Cell cellCurrent;
        public override void OnPickup()
        {
            base.OnPickup();
            piece.StartGrab(null);
        }
        public override void OnDrop()
        {
            base.OnDrop();
            if (cellCurrent != null) {
                piece.StopGrab(cellCurrent.cell);
            } else {
                piece.StopGrab(null);
            }
            
        }
        public void AddCell(RaycastButton.Cell cell) {
            RemoveCell();
            cellCurrent = cell;
            cell.OnRaycastEnter();
        }
        public void RemoveCell() {
            if (cellCurrent != null) {
                cellCurrent.OnRaycastExit();
            }
            cellCurrent = null;
        }
    }
}
