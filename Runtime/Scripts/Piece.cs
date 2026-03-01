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
        public Vector2Int[] dir2 = new Vector2Int[27];
        public byte dir1Count = 0;
        public bool isBlack = false;
        public byte isNotMoved = 0;
        public byte isCaptured = 0;
        bool isMoved = false;
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
            PerformShowMove();
        }
        public void StopGrab(Cell cell) {
            if (cell == null) {
                cell = gameField.cells[position];
            }
            if (cell.isCanMoveHere == false) {
                cell = gameField.cells[position];
            }
            pieceGrab.StopGrab();

            if (cell != GetCurrentCell()) {
                isMoved = true;
                GetPiece().PerformMove(cell,this);
                GetPiece().AfterMove(cell,this);
            } else {
                cell.PlacePiece(this);
            }
        }
        virtual public void PerformMove(Cell cell, Piece piece) {
            Cell cellOld = piece.GetCurrentCell();
            cellOld.VectorGetPieces();
            cell.VectorGetPieces();

            piece.GetPiece().CalcAttack(piece, true);
            cellOld.VectorCalcAttack(true);
            cell.VectorCalcAttack(true);

            piece.isNotMoved = 1;
            if (cell.pieceCurrent != null) {
                cell.pieceCurrent.isCaptured = 1;
                cell.pieceCurrent.PerformCapture();
            }
            
            piece.positionPrevious = piece.position;
            piece.gameField.cells[piece.position].pieceCurrent = null;
            piece.position = cell.position;
            
            piece.gameField.HideMove();
            piece.gameField.ChangeSide();

            cell.PlacePiece(piece);
            
            piece.GetPiece().CalcAttack(piece, false);
            cellOld.VectorCalcAttack(false);
            cell.VectorCalcAttack(false);
        }
        virtual public void AfterMove(Cell cell, Piece piece) {
            if (piece.isMoved == true) {
                piece.isMoved = false;
            }
        }
        public void PlacePiece(Cell cell, Piece piece) {
            piece.positionPrevious = piece.position;
            piece.gameField.cells[piece.position].pieceCurrent = null;
            piece.position = cell.position;
            piece.isNotMoved = 1;
            piece.isMoved = true;
            cell.PlacePiece(piece);
            piece.GetPiece().CalcAttack(piece);
            piece.GetPiece().AfterMove(cell, piece);
        }

        public void PerformCapture() {
            if (isCaptured == 1) {
                GetPiece().CalcAttack(this, true);
                meshRenderer.enabled = false;
                GetCurrentCell().pieceCurrent = null;
                //RemoveAttack();
            }
        }
        public virtual void CalcAttack(Piece piece, bool isRemove = false) {

        }
        public void AddAttack(Cell cell, bool isRemove = false) {
            if (isRemove) dir1Count = 0;
            cell.SetAttack(this, !isRemove);
            dir1[dir1Count] = cell.position;
            dir1Count++;
        }
        public void AddAttack(Cell cell, Vector2Int dir, bool isRemove = false) {
            if (isRemove) dir1Count = 0;
            cell.SetAttack(this, !isRemove);
            dir1[dir1Count] = cell.position;
            dir1Count++;
            cell.SetAttackVector(dir, !isRemove);
        }
        virtual public void RemoveAttack(Piece piece) {
            piece.GetPiece().CalcAttack(piece, false);
            /*
            for(int i = 0; i < piece.dir1Count; i++) {
                piece.gameField.cells[piece.dir1[i]].SetAttack(piece, false);
            }
            
            piece.dir1Count = 0;
            */
        }
        public void AddCellAttack(Vector2Int dir, bool isRemove = false) {
            Cell cell = GetCurrentCell().GetNeighbourByOffset(dir);
            if (cell != null) {
                AddAttack(cell, isRemove);
            }
        }
        public void AddSlidingCellAttack(Vector2Int dir, bool isRemove = false) {
            Cell[] cells = GetCurrentCell().GetSlidingCells(dir);
            foreach (Cell cell in cells) {
                //dir2[dir1Count] = dir;
                AddAttack(cell, dir, isRemove);
                //cell.SetAttackVector(dir, true);
            }
        }
        virtual public void ShowMove(Piece piece) {
            for (int i = 0; i < piece.dir1Count; i++) {
                piece.gameField.AddMove(piece.gameField.cells[piece.dir1[i]], piece);
            }
        }
        public void PerformShowMove() {
            GetPiece().ShowMove(this);
        }
        public Piece GetPiece() {
            return gameField.pieces.GetPiece(indexType);
        }
    }
}
