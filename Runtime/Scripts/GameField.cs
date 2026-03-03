using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using UnityEditor.Experimental.GraphView;
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
        public Cell[] cellChanged = new Cell[50];
        public byte cellChangedCount = 0;
        public Piece[] piecesToRemove = new Piece[10];
        public byte piecesToRemoveCount = 0;
        public Piece[] piecesMove = new Piece[10];
        public Cell[] piecesMoveDestination = new Cell[10];
        public byte piecesMoveCount = 0;
        public byte isKingCheck = 0;
        public Cell[] cellsNeedDefend = new Cell[27];
        public byte cellsNeedDefendCount = 0;
        public byte pieceAttackKing;
        
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
        public void AddMove(Cell cell, bool ignoreKingCheck = false) {
            if (cell.SetMove(true, IsKingCheck(ignoreKingCheck) == true)) {
                dirMove[dirMoveCount] = cell.position;
                dirMoveCount++;
            }
        }
        public void AddMove(Cell cell, Piece piece, bool ignoreKingCheck = false) {
            if (cell.SetMove(piece, IsKingCheck(ignoreKingCheck)) == true) {
                dirMove[dirMoveCount] = cell.position;
                dirMoveCount++;
                //PerformCheckIsKing(cell)
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
                cells[dirMove[i]].SetMove(piece, IsKingCheck());
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
        void AddCellChangedArray(Cell cell) {
            cellChanged[cellChangedCount] = cell;
            cellChangedCount++;
        }
        public void ResetChangedCell() {
            for (int i = 0; i < cellChangedCount; i++) {
                Piece piece =cellChanged[i].pieceCurrent;
                if (piece != null) piece.isCalculatedAttacks = 0;
            }
            cellChangedCount = 0;
            piecesToRemoveCount = 0;
            piecesMoveCount = 0;
        }
        public void AddChangedCell(Piece piece, Cell cell) {
            cell.VectorGetPieces(piece);
            AddCellChangedArray(cell);
            cell.SetMaterial(0);
        }
        public void AddChangedCell(Cell cell) {
            AddCellChangedArray(cell);
        }
        public Material materialgreen;
        public void UpdateChangedCells(bool isRemove = false) {
            Piece piece;
            byte isCalculatedAttacks;
            if (isRemove) isCalculatedAttacks = 0;
            else isCalculatedAttacks = 1;
            for (int i = 0; i < cellChangedCount; i++) {
                piece = cellChanged[i].pieceCurrent;
                if (piece != null) {
                    if (piece.isCalculatedAttacks == isCalculatedAttacks) {
                        piece.isCalculatedAttacks++;
                        cellChanged[i].meshRenderer.material = materialgreen;
                        piece.GetPiece().CalcAttack(piece, isRemove, false);
                    }
                }
                //cellChanged[i].isCalculatedAttacks++;
            }
        }
        public void AddRemovePiece(Piece piece) {
            AddChangedCell(piece, piece.GetCurrentCell());
            piecesToRemove[piecesToRemoveCount] = piece;
            piecesToRemoveCount++;
        }
        public void AddChangePosition(Piece piece, Cell destination) {
            //piecesMove[piecesMoveCount] = new PiecesMove();
            AddChangedCell(piece, piece.GetCurrentCell());
            AddChangedCell(piece, destination);
            piecesMove[piecesMoveCount]= piece;
            piecesMoveDestination[piecesMoveCount] = destination;
            piecesMoveCount++;
        }
        public void UpdateRemovePiece() {
            for (int i = 0; i < piecesToRemoveCount; i++) {
                piecesToRemove[i].GetCurrentCell().isCalculatedAttacks = 1;
                piecesToRemove[i].isCaptured = 1;
                piecesToRemove[i].PerformCapture();
            }
        }
        public void UpdateChangePosition() {
            for (int i = 0; i < piecesMoveCount; i++) {
                Debug.Log("place to " + piecesMoveDestination[i].name);
                piecesMove[i].PlacePiece(piecesMoveDestination[i]);
            }
        }
        public void MakeMove() {
            pieceAttackKing = byte.MaxValue;
            UpdateChangedCells(true);
            UpdateRemovePiece();
            UpdateChangePosition();
            UpdateChangedCells(false);
            ChangeSide();
            ResetChangedCell();
            ResetCellsCheck();
            CheckKing();
        }
        public void CheckKing() {
            if (pieceAttackKing == byte.MaxValue) return;
            Piece piece = pieces.InTableAll[pieceAttackKing];
        
            foreach (Piece king in pieces.allKings) {
                if(king.GetCurrentCell().IsAttacking(king)) {
                    king.GetCurrentCell().VectorGetPieces(king, true);
                    break;
                }
            }
            AddCellCheck(piece.GetCurrentCell());
            isKingCheck = 1;
            
            SetCellsToCheck();
            Debug.Log("CHECK");
            
        }
        public void AddCellCheck(Cell cell) {
            cellsNeedDefend[cellsNeedDefendCount] = cell;
            cellsNeedDefendCount++;
        }
        public void SetCellsToCheck() {
            for (int i = 0; i < cellsNeedDefendCount; i++) {
                cellsNeedDefend[i].isCheck = true;
            }
            if (cellsNeedDefendCount == 0) {

            }
        }
        public void ResetCellsCheck() {
            for (int i = 0; i < cellsNeedDefendCount; i++) {
                cellsNeedDefend[i].isCheck = false;
            }
            cellsNeedDefendCount = 0;
            isKingCheck = 0;
        }
        public void PerformCheckIsKing(Cell cell, Piece piece) {
            if (cell.pieceCurrent == null) return;
            if (cell.pieceCurrent.GetPiece() == pieces.king) {
                if (cell.pieceCurrent.isBlack != piece.isBlack) {
                    AddAttackPieceKing(piece);
                    Debug.Log(piece.GetCurrentCell().name + "is attacking");
                }
                    
            }
        }
        public bool IsKingCheck(bool ignore = false) {
            if (ignore) return false;
            if (isKingCheck == 0) return false;
            if (isKingCheck == 1) return true;
            return false;
        }
        public void AddAttackPieceKing(Piece piece) {
            pieceAttackKing = piece.id;
        }
    }
}
