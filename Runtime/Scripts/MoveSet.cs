using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.UdonNetworkCalling;
namespace Andrey04o.Chess {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MoveSet : UdonSharpBehaviour
    {
        public byte indexType;
        //public Mesh mesh;
        public MeshFilter meshFilter;
        public Vector3 offset;
        public Sprite spriteBlack;
        public Sprite spriteWhite;

        virtual public void PerformMove(Cell cell, Piece piece) {
            
            if (Networking.IsOwner(Networking.LocalPlayer, piece.gameField.gameObject) == false) {
                NetworkCalling.SendCustomNetworkEvent((IUdonEventReceiver)piece.gameField, NetworkEventTarget.Owner, nameof(GameField.PerformMoveNetwork), cell.position, piece.id);
                cell.PlacePiece(piece);
                return;
            }
            
            if (cell.pieceCurrent != null) {
                piece.gameField.AddRemovePiece(cell.pieceCurrent);
            }
            piece.gameField.SetPreviousPosition(piece);
            piece.gameField.AddChangePosition(piece, cell);
            piece.gameField.SetPosition(cell);

            piece.gameField.MakeMove();
        }
        virtual public void AfterMove(Cell cell, Piece piece) {
            if (piece.isMoved == true) {
                piece.isMoved = false;
            }
        }
        public virtual void CalcAttack(Piece piece, bool isRemove = false, bool isVisualMoving = false) {

        }

        virtual public void RemoveAttack(Piece piece) {
            piece.GetPiece().CalcAttack(piece, false);
        }
        virtual public void ShowMove(Piece piece) {
            piece.GetPiece().CalcAttack(piece, false, true);
        }
    }
}
