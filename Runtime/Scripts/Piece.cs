using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDK3.Components;
using Andrey04o.RaycastButton;
using System;
using System.Linq;
namespace Andrey04o.Chess {
    public class Piece : UdonSharpBehaviour
    {
        public byte id;
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        public byte indexType;
        public Vector3 offset;
        public GameField gameField;
        public PieceGrab pieceGrab;
        public VRCPickup pickup;
        public VRCObjectSync objectSync;
        [UdonSynced] public byte position;
        public byte positionPrevious;
        public byte[] dir1 = new byte[27];
        //public Vector2Int[] dir2 = new Vector2Int[27];
        public byte dir1Count = 0;
        public bool isBlack = false;
        public byte isNotMoved = 0;
        public byte isCaptured = 0;
        bool isMoved = false;
        public byte isCalculatedAttacks = 0;
        public byte countPossibleMoves = 0;
        public Cell GetCurrentCell() {
            return gameField.cells[position];
        }
        public Cell GetCurrentCellPrevious() {
            return gameField.cells[positionPrevious];
        }
        public void ChangeType(byte indexType) {
            Piece piece = gameField.pieces.GetPiece(indexType);
            if (piece != null) {
                meshFilter.sharedMesh = piece.meshFilter.sharedMesh;
                offset = piece.offset;
                meshRenderer.gameObject.transform.localPosition = offset;
                this.indexType = indexType;
            }
        }
        public void StartGrab(TileRaycastHandler handler) {
            if (gameField.IsHisTurn(this) == false) return;
            handler.currentPiece = this;
            pieceGrab.StartGrab(handler.cursorController);
            //gameField.RemoveAttack();
            //gameField.CalcAttacks();
            GetPiece().ShowMove(this);
        }
        public void StopGrab(Cell cell) {
            if (cell == null) {
                cell = gameField.cells[position];
            }
            if (cell.isCanMoveHere == false) {
                cell = gameField.cells[position];
            }
            pieceGrab.StopGrab();
            gameField.HideMove();
            if (cell != GetCurrentCell()) {
                isMoved = true;
                GetPiece().PerformMove(cell,this);
                GetPiece().AfterMove(cell,this);
            } else {
                cell.PlacePiece(this);
            }
        }
        virtual public void PerformMove(Cell cell, Piece piece) {
            if (cell.pieceCurrent != null) {
                piece.gameField.AddRemovePiece(cell.pieceCurrent);
            }
            piece.gameField.AddChangePosition(piece, cell);

            piece.gameField.MakeMove();
        }
        virtual public void AfterMove(Cell cell, Piece piece) {
            if (piece.isMoved == true) {
                piece.isMoved = false;
            }
        }
        public void PlacePiece(Cell cell) {
            positionPrevious = position;
            gameField.cells[position].pieceCurrent = null;
            position = cell.position;
            isNotMoved = 1;
            //isMoved = true;
            cell.PlacePiece(this);
        }

        public void PerformCapture() {
            if (isCaptured == 1) {
                //GetPiece().CalcAttack(this, true);
                meshRenderer.enabled = false;
                GetCurrentCell().pieceCurrent = null;
                //RemoveAttack();
            }
        }
        public virtual void CalcAttack(Piece piece, bool isRemove = false, bool isVisualMoving = false) {

        }
        public void AddAttack(Cell cell, bool isRemove = false, bool isVisualMoving = false, bool ignoreKingCheck = false) {
            cell.SetAttack(this, !isRemove, isVisualMoving, ignoreKingCheck);
        }
        public void AddAttack(Cell cell, Vector2Int dir, bool isRemove = false, bool isVisualMoving = false, bool ignoreKingCheck = false) {
            cell.SetAttack(this, !isRemove, isVisualMoving, ignoreKingCheck);
            if (isVisualMoving == false) cell.SetAttackVector(dir, !isRemove);
        }
        virtual public void RemoveAttack(Piece piece) {
            piece.GetPiece().CalcAttack(piece, false);
        }
        public void AddCellAttack(Vector2Int dir, bool isRemove = false, bool isVisualMoving = false, bool ignoreKingCheck = false) {
            Cell cell = GetCurrentCell().GetNeighbourByOffset(dir);
            if (cell != null) {
                AddAttack(cell, isRemove, isVisualMoving, ignoreKingCheck);
            }
        }
        public void AddSlidingCellAttack(Vector2Int dir, bool isRemove = false, bool isVisualMoving = false, bool ignoreKingCheck = false) {
            Cell[] cells = GetCurrentCell().GetSlidingCells(dir);
            foreach (Cell cell in cells) {
                //dir2[dir1Count] = dir;
                AddAttack(cell, dir, isRemove, isVisualMoving, ignoreKingCheck);
                //cell.SetAttackVector(dir, true);
            }
        }
        virtual public void ShowMove(Piece piece) {
            piece.GetPiece().CalcAttack(piece, false, true);
            //for (int i = 0; i < piece.dir1Count; i++) {
            //    piece.gameField.AddMove(piece.gameField.cells[piece.dir1[i]], piece);
            //}
        }
        public Piece GetPiece() {
            return gameField.pieces.GetPiece(indexType);
        }
    }
}
