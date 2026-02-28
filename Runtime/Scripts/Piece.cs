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
        public byte dir1Count = 0;
        public bool isBlack = false;
        public byte isNotMoved = 0;
        public byte isCaptured = 0;
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
            gameField.RemoveAttack();
            gameField.CalcAttacks();
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
                GetPiece().PerformMove(cell,this);
                //gameField.HideMove();
                //gameField.ChangeSide();
            }
            //gameField.cells[position].pieceCurrent = null;
            cell.PlacePiece(this);
        }
        virtual public void PerformMove(Cell cell, Piece piece) {
            //gameField.cells[position].attackBy
            //RemoveAttack();
            piece.positionPrevious = piece.position;
            piece.gameField.cells[piece.position].pieceCurrent = null;
            piece.position = cell.position;
            
            piece.isNotMoved = 1;
            if (cell.pieceCurrent != null) {
                cell.pieceCurrent.isCaptured = 1;
                cell.pieceCurrent.PerformCapture();
            }
            piece.gameField.HideMove();
            piece.gameField.ChangeSide();
        }
        public void PlacePiece(Cell cell, Piece piece) {
            piece.positionPrevious = piece.position;
            piece.gameField.cells[piece.position].pieceCurrent = null;
            piece.position = cell.position;
            piece.isNotMoved = 1;
            cell.PlacePiece(piece);
        }

        public void PerformCapture() {
            if (isCaptured == 1) {
                meshRenderer.enabled = false;
                GetCurrentCell().pieceCurrent = null;
                //RemoveAttack();
            }
        }
        public virtual void CalcAttack(Piece piece) {

        }
        public void AddAttack(Cell cell) {
            cell.SetAttack(this, true);
            Debug.Log(cell.position);
            dir1[dir1Count] = cell.position;
            dir1Count++;
        }
        void AddAttack(Cell[] cells) {
            foreach (Cell cell in cells) {
                AddAttack(cell);
            }
        }
        public void RemoveAttack() {
            //for(int i = 0; i < dir1Count; i++) {
            //    gameField.cells[dir1[i]].SetAttack(this, false);
            //}
            //dir1[dir1Count] = byte.MaxValue;
            
            dir1Count = 0;
        }
        public void AddCellAttack(Vector2Int dir) {
            Cell cell = GetCurrentCell().GetNeighbourByOffset(dir);
            if (cell != null) {
                AddAttack(cell);
            }
        }
        public void AddCellAttackKing(Vector2Int dir) {
            Cell cell = GetCurrentCell().GetNeighbourByOffset(dir);
            if (cell != null) {
                if (cell.attackByCount == 0) {
                    AddAttack(cell);
                }
            }
        }
        public void AddSlidingCellAttack(Vector2Int dir) {
            AddAttack(GetCurrentCell().GetSlidingCells(dir));
        }
        virtual public void ShowMove(Piece piece) {
            for (int i = 0; i < piece.dir1Count; i++) {
                piece.gameField.AddMove(piece.gameField.cells[piece.dir1[i]], piece);
            }
        }
        public void PerformShowMove() {
            GetPiece().ShowMove(this);
        }
        public void PerformCalcAttack() {
            GetPiece().CalcAttack(this);
        }
        public Piece GetPiece() {
            return gameField.pieces.GetPiece(indexType);
        }
    }
}
