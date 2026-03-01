using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace Andrey04o.Chess {
    public class GameField : UdonSharpBehaviour
    {
        public Pieces pieces;
        public Line[] lines;
        public Cell[] cells;
        public byte indexSideTurn = 0;
        byte[] dirMove = new byte[27];
        byte dirMoveCount = 0;
        byte enPassant = byte.MaxValue;
        public Piece enPassantPiece;
        public Cell enPassantCell;
        bool isAttackCalc = false;
        public bool IsHisTurn(Piece piece) {
            if (indexSideTurn == 0) {
                if (piece.isBlack == false) {
                    return true;
                }
            }
            if (indexSideTurn == 1) {
                if (piece.isBlack == true) {
                    return true;
                }
            }
            return false;
        }
        public void ChangeSide() {
            indexSideTurn++;
            indexSideTurn = (byte)(indexSideTurn % 2);
            RemoveEnPassant();
        }
        public void CalcAttacks() {
            if (isAttackCalc == true) return;
            isAttackCalc = true;
            foreach (Piece piece in pieces.InTableAll) {
                piece.GetPiece().CalcAttack(piece);
            }
        }
        public void RemoveAttack() {
            isAttackCalc = false;
            foreach (Piece piece in pieces.InTableAll) {
                piece.GetPiece().RemoveAttack(piece);
            }
            foreach (Cell cell in cells) {
                cell.RemoveAttack();
            }
        }
        public void AddMove(Cell cell) {
            cell.SetMove(true);
            dirMove[dirMoveCount] = cell.position;
            dirMoveCount++;
        }
        public void AddMove(Cell cell, Piece piece) {
            if (cell.SetMove(piece) == true) {
                dirMove[dirMoveCount] = cell.position;
                dirMoveCount++;
            }
        }
        public void AddMove(Cell[] cells, Piece piece) {
            foreach (Cell cell in cells) {
                AddMove(cell, piece);
            }
        }
        public void HideMove() {
            for(int i = 0; i < dirMoveCount; i++) {
                cells[dirMove[i]].RemoveMove();
            }
            dirMoveCount = 0;
        }
        void ShowMove(Piece piece) {
            for(int i = 0; i < dirMoveCount; i++) {
                cells[dirMove[i]].SetMove(piece);
            }
        }
        public void SetEnPassant(Piece piece) {
            enPassant = piece.id;
            PerformEnPassant();
            RequestSerialization();
        }
        public void RemoveEnPassant() {
            if (enPassant == byte.MaxValue) return;
            enPassantCell.pieceEnPassant = null;
            enPassant = byte.MaxValue;
            PerformEnPassant();
            RequestSerialization();
        }
        public bool ShowEnPassant() {
            if (enPassant == byte.MaxValue) return false;
            //enPassantCell.pieceCurrent = enPassantPiece;
            enPassantCell.pieceEnPassant = enPassantPiece;
            return true;
            //AddMove(enPassantCell);
        }
        public void PerformEnPassant() {
            if (enPassant == byte.MaxValue) {
                enPassantPiece = null;
                enPassantCell = null;
                return;
            }
            enPassantPiece = pieces.InTableAll[enPassant];
            enPassantCell = cells[enPassantPiece.position];
            if (enPassantPiece.isBlack) enPassantCell = enPassantCell.GetNeighbour(Vector2Int.down);
            else enPassantCell = enPassantCell.GetNeighbour(Vector2Int.up);
        }
        //public void Get
        public override void OnDeserialization()
        {
            base.OnDeserialization();
            PerformEnPassant();
        }
    }
}
