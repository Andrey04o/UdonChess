using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.Udon.Common;
using VRC.SDK3.UdonNetworkCalling;
using Andrey04o.RaycastButton;
namespace Andrey04o.Chess {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class GameField : UdonSharpBehaviour
    {
        public Pieces pieces;
        public Line[] lines;
        public Cell[] cells;
        [UdonSynced] public byte indexSideTurn = 0;
        byte[] dirMove = new byte[27];
        byte dirMoveCount = 0;
        [UdonSynced] byte enPassant = byte.MaxValue;
        [UdonSynced] public byte[] syncData = new byte[512];
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
        [UdonSynced] public byte isKingCheck = 0;
        public Cell[] cellsNeedDefend = new Cell[27];
        public Cell[] cellsNeedDefend2 = new Cell[27];
        public Cell[] cellsInAttack = new Cell[8];
        public byte cellsNeedDefendCount = 0;
        public byte cellsNeedDefendCount2 = 0;
        public byte cellsInAttackCount = 0;
        [UdonSynced] public byte pieceAttackKing;
        [UdonSynced] public byte isStalemate = 0;
        [UdonSynced] public byte promotionPiece = byte.MaxValue;
        [UdonSynced] public byte promotionDestination = byte.MaxValue;
        public TileRaycastHandler tileRaycastHandler;
        
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
            if (cell.SetMove(true, IsKingCheck(ignoreKingCheck), IsUnderAttack())) {
                dirMove[dirMoveCount] = cell.position;
                dirMoveCount++;
                Debug.Log("addmove " + cell.name + " " + dirMoveCount);
            }
        }
        bool isKingBeDanger = false;
        public void CheckKingSafe(Piece piece) {
            piece.GetCurrentCell().VectorCheckKing();
        }
        public void AddMove(Cell cell, Piece piece, bool ignoreKingCheck = false) {
            //CheckKingSafe(cell, piece);
            //SetCellsToCheck2();
            if (cell.SetMove(piece, IsKingCheck(ignoreKingCheck),IsUnderAttack())) {
                dirMove[dirMoveCount] = cell.position;
                dirMoveCount++;
                Debug.Log("addmove " + cell.name + " " + dirMoveCount);
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
                cells[dirMove[i]].SetMove(piece, IsKingCheck(), false);
            }
        }
        public void SetEnPassant(Piece piece) {
            enPassant = piece.id;
            PerformEnPassant();
        }
        public void RemoveEnPassant() {
            if (enPassant == byte.MaxValue) return;
            enPassantCell.pieceEnPassant = null;
            enPassant = byte.MaxValue;
            PerformEnPassant();
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
            UnpackSyncData();
            PerformEnPassant();
            if (isStalemate == 0) return;
            if (isStalemate == 1) {
                Debug.Log("STALEMATE");
            } else {
                Debug.Log("CHECKMATE");
            }
        }
        void AddCellChangedArray(Cell cell) {
            cellChanged[cellChangedCount] = cell;
            cellChangedCount++;
        }
        public void ResetChangedCell() {
            for (int i = 0; i < cellChangedCount; i++) {
                Piece piece = cellChanged[i].pieceCurrent;
                
                if (piece != null) {
                    piece.isCalculatedAttacks = 0;
                }
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
                        piece.GetPiece().CalcAttack(piece, isRemove, false);
                    }
                }
                cellChanged[i].UpdateInfo();
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
            Debug.Log("addchangepost");
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
            isStalemate = 0;
            UpdateChangedCells(true);
            UpdateRemovePiece();
            UpdateChangePosition();
            UpdatePromotion();
            UpdateChangedCells(false);
            ChangeSide();
            ResetChangedCell();
            ResetCellsCheck();
            CheckKing();
            CheckStalemate();
            PerformGameOver();
            PackSyncData();
            RequestSerialization();
        }
        
        void PackSyncData() {
            int offset = 0;
            
            // Pack pieces data
            foreach (Piece piece in pieces.InTableAll) {
                syncData[offset++] = piece.indexType;
                syncData[offset++] = piece.position;
                syncData[offset++] = piece.isNotMoved;
                syncData[offset++] = piece.isCaptured;
            }
            
            // Pack cells data
            foreach (Cell cell in cells) {
                syncData[offset++] = cell.attackByCount;
                syncData[offset++] = cell.attackByCountBlack;
                syncData[offset++] = cell.attackVector;
            }
        }
        void UnpackSyncData() {
            int offset = 0;
            tileRaycastHandler.currentPiece = null;
            HideMove();
            foreach (Cell cell in cells) {
                cell.pieceCurrent = null;
                cell.pieceEnPassant = null;
            }
            // Unpack pieces data
            foreach (Piece piece in pieces.InTableAll) {
                piece.pieceGrab.StopGrab();
                piece.indexType = syncData[offset++];
                piece.position = syncData[offset++];
                piece.isNotMoved = syncData[offset++];
                piece.isCaptured = syncData[offset++];
                piece.PerformCapture(false);
                if (piece.isCaptured == 0) {
                    cells[piece.position].PlacePieceLocal(piece);
                    cells[piece.position].pieceCurrent = piece;
                }
                    
            }
            
            // Unpack cells data
            foreach (Cell cell in cells) {
                cell.attackByCount = syncData[offset++];
                cell.attackByCountBlack = syncData[offset++];
                cell.attackVector = syncData[offset++];
                cell.OnDeserialization();
            }
        }
        public void CheckStalemate() {
            //isStalemateCheck = true;
            Piece piece;
            for (int i = 0; i < pieces.InTableAll.Length; i++) {
                piece = pieces.InTableAll[i];
                
                if (IsHisTurn(piece) == false) {
                    continue;
                    }
                if (piece.isCaptured == 1) {
                    continue;
                    }
                piece.ShowMove(piece);
                if (dirMoveCount != 0) {
                    Debug.Log("no stalemate");
                    HideMove();
                    return;
                }
            }
            isStalemate = 1;
            Debug.Log("stalemate 1, side " + indexSideTurn);
            //ResetStalemate();
        }
        public void CheckKing() {
            if (pieceAttackKing == byte.MaxValue) return;
            Piece piece = pieces.InTableAll[pieceAttackKing];
            byte attack;
            foreach (Piece king in pieces.allKings) {
                attack = king.GetCurrentCell().CountAttack(king);
                if (attack > 1) {
                    isKingCheck = 1; 
                    return; // only king moves are possible
                }
                if (attack > 0) {
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
        public void AddCellCheck2(Cell cell) {
            cellsNeedDefend2[cellsNeedDefendCount2] = cell;
            cellsNeedDefendCount2++;
        }
        public void AddCellInAttack(Cell cell) {
            cellsInAttack[cellsInAttackCount] = cell;
            cellsInAttackCount++;
        }
        public void SetCellsToCheck() {
            for (int i = 0; i < cellsNeedDefendCount; i++) {
                cellsNeedDefend[i].isCheck = true;
            }
        }
        public void SetCellsToCheck2() {
            for (int i = 0; i < cellsNeedDefendCount2; i++) {
                cellsNeedDefend2[i].isCheck2 = true;
            }
        }
        public void SetCellsInAttack() {
            for (int i = 0; i < cellsInAttackCount; i++) {
                cellsInAttack[i].isInAttack = true;
            }
        }
        public void ResetCellsCheck() {
            for (int i = 0; i < cellsNeedDefendCount; i++) {
                cellsNeedDefend[i].isCheck = false;
            }
            cellsNeedDefendCount = 0;
            isKingCheck = 0;
        }
        public void ResetCellsCheck2() {
            Debug.Log(cellsNeedDefendCount2);
            for (int i = 0; i < cellsNeedDefendCount2; i++) {
                cellsNeedDefend2[i].isCheck2 = false;
            }
            cellsNeedDefendCount2 = 0;
        }
        public void ResetCellsInAttack() {
            for (int i = 0; i < cellsInAttackCount; i++) {
                cellsInAttack[i].isInAttack = false;
            }
            cellsInAttackCount = 0;
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
            //if (cellsNeedDefendCount2 > 0) return false;
            if (isKingCheck == 0) return false;
            if (isKingCheck == 1) return true;
            return false;
        }
        public bool IsUnderAttack() {
            if (cellsNeedDefendCount2 > 0) return true;
            return false;
        }
        public void AddAttackPieceKing(Piece piece) {
            pieceAttackKing = piece.id;
        }
        public void PerformGameOver() {
            if (isStalemate == 0) return;
            foreach (Piece king in pieces.allKings) {
                if(king.GetCurrentCell().IsAttacking(king)) {
                    if(isStalemate == 1) {
                        isStalemate = 2;
                    }
                    break;
                }
            }
            if (isStalemate == 1) {
                Debug.Log("STALEMATE");
            } else {
                Debug.Log("CHECKMATE");
            }
        }
        public void SetPromotion(Piece piece, byte destination) {
            promotionPiece = piece.id;
            promotionDestination = destination;
        }
        public void CancelPromotion() {
            if (promotionPiece != byte.MaxValue)
            pieces.InTableAll[promotionPiece].CancelPromotion();
            promotionPiece = byte.MaxValue;
        }
        public void ConfirmPromotion(byte id) {
            promotionNewType = id;
            Piece piece = pieces.InTableAll[promotionPiece];
            Cell cell = cells[promotionDestination];
            if (cell.pieceCurrent != null) AddRemovePiece(cell.pieceCurrent);
            AddChangePosition(piece, cell);
            MakeMove();
            //piece.GetPiece().PerformMove(cell,piece);
            //piece.GetPiece().AfterMove(cell,piece);
            promotionPiece = byte.MaxValue;
        }
        byte promotionNewType;
        public void AddPromotion(Piece piece, byte newType) {
            promotionPiece = piece.id;
            promotionNewType = newType;
        }
        public void UpdatePromotion() {
            if (promotionPiece != byte.MaxValue) {
                Piece piece = pieces.InTableAll[promotionPiece];
                //Cell currentCell = piece.GetCurrentCell();
                piece.ChangeType(promotionNewType);
                piece.ShowPromotion(false, byte.MaxValue);
                //currentCell.GetNeighbour(forward*-1).PlacePieceLocal(this);
                //gameField.ConfirmPromotion();
            }
        }
        public void Set2DView(bool value, Quaternion rotation) {
            foreach(Piece piece in pieces.InTableAll) {
                piece.Set2DMode(value, rotation);
            }
        }
        public void Set2DView(bool value) {
            foreach(Piece piece in pieces.InTableAll) {
                piece.Set2DMode(value);
            }
        }
        [NetworkCallable] public void PerformMoveNetwork(byte cellId, byte pieceId) {
            Cell cell = cells[cellId];
            Piece piece = pieces.InTableAll[pieceId];
            piece.GetPiece().PerformMove(cell, piece);
        }
    }
}
