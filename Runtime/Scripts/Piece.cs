using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDK3.Components;
using Andrey04o.RaycastButton;
using System;
using System.Linq;
using VRC.SDK3.Dynamics.Constraint.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.UdonNetworkCalling;
namespace Andrey04o.Chess {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Piece : UdonSharpBehaviour
    {
        public byte id;
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        public MeshCollider meshCollider;
        [UdonSynced] public byte indexType;
        public Vector3 offset;
        public Vector2Int forward;
        public Vector2Int left;
        public GameField gameField;
        public PieceGrab pieceGrab;
        public VRCPickup pickup;
        public VRCObjectSync objectSync;
        [UdonSynced] public byte position;
        public byte positionPrevious;
        public bool isBlack = false;
        [UdonSynced] public byte isNotMoved = 0;
        [UdonSynced] public byte isCaptured = 0;
        bool isMoved = false;
        public byte isCalculatedAttacks = 0;
        public byte countPossibleMoves = 0;
        public Promotion promotion;
        public byte isShowPromotion;
        public SpriteRenderer spriteRenderer;
        public Sprite spriteWhite;
        public Sprite spriteBlack;
        public VRCRotationConstraint rotationConstraint;
        public bool is2DMode = false;
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
                meshCollider.sharedMesh = piece.meshFilter.sharedMesh;
                offset = piece.offset;
                meshRenderer.gameObject.transform.localPosition = offset;
                this.indexType = indexType;
                if (isBlack) spriteRenderer.sprite  = piece.spriteBlack;
                else spriteRenderer.sprite = piece.spriteWhite;
            }
        }
        public void StartGrab(TileRaycastHandler handler) {
            //if (Networking.IsOwner(Networking.LocalPlayer, gameObject) == false)
            //    Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            if (gameField.IsHisTurn(this) == false) return;
            if (handler != null) handler.currentPiece = this;
            gameField.CancelPromotion();
            if (handler != null) pieceGrab.StartGrab(handler.cursorController);
            gameField.CheckKingSafe(this);
            gameField.SetCellsToCheck2();
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
            gameField.ResetCellsCheck2();
            if (cell != GetCurrentCell()) {
                isMoved = true;
                if (Networking.IsOwner(Networking.LocalPlayer, gameField.gameObject)) {
                    GetPiece().PerformMove(cell,this);
                    GetPiece().AfterMove(cell,this);
                } else {
                    SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(PerformMoveNetwork), cell.position, this.id);
                    //cell.PlacePieceLocal(this);
                    //RequestSerialization();
                }
            } else {
                cell.PlacePiece(this);
            }
        }
        public void PromotionCheck() {
            if (GetPiece() == gameField.pieces.pawn) {

            }
        }
        public void ShowPromotion(bool value, byte destination) {
            if (value == true) isShowPromotion = 1;
            else isShowPromotion = 0;
            gameField.SetPromotion(this, destination);
            promotion.gameObject.SetActive(value);
        }
        public void CancelPromotion() {
            ShowPromotion(false, byte.MaxValue);
            GetCurrentCell().PlacePiece(this);
            gameField.promotionPiece = byte.MaxValue;
        }
        public void ConfirmPromotion(byte id) {
            Cell currentCell = GetCurrentCell();
            //gameField.AddPromotion()
            gameField.ConfirmPromotion(id);
            
            //currentCell.GetNeighbour(forward*-1).PlacePieceLocal(this);
            
        }
        virtual public void PerformMove(Cell cell, Piece piece) {
            if (cell.pieceCurrent != null) {
                piece.gameField.AddRemovePiece(cell.pieceCurrent);
            }
            piece.gameField.AddChangePosition(piece, cell);

            piece.gameField.MakeMove();
        }
        [NetworkCallable] public void PerformMoveNetwork(byte cellId, byte pieceId) {
            Cell cell = gameField.cells[cellId];
            Piece piece = gameField.pieces.InTableAll[pieceId];
            piece.GetPiece().PerformMove(cell, piece);
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
                meshCollider.enabled = false;
                spriteRenderer.gameObject.SetActive(false);
                GetCurrentCell().pieceCurrent = null;
                //RemoveAttack();
            } else {
                if (is2DMode) {
                    spriteRenderer.gameObject.SetActive(true);
                } else {
                    meshRenderer.enabled = true;
                }
                meshCollider.enabled = true;
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
        public void Set2DMode(bool value, Quaternion rotation) {
            is2DMode = value;
            if (isCaptured == 0) {
                spriteRenderer.gameObject.SetActive(value);
                //spriteRenderer.transform.rotation = rotation;
                if (value == true) {
                    rotationConstraint.ActivateConstraint();
                    promotion.ChangeRotation(rotation);
                }
                else {
                    rotationConstraint.ZeroConstraint();
                    promotion.ResetRotation();
                }
                meshRenderer.enabled = !value;
            }
        }
        public void Set2DMode(bool value) {
            is2DMode = value;
            if (isCaptured == 0) {
                spriteRenderer.gameObject.SetActive(value);
                promotion.ResetRotation();
                meshRenderer.enabled = !value;
            }
        }
        public override void OnDeserialization()
        {
            base.OnDeserialization();
            PerformCapture();
            //if (Networking.IsOwner(Networking.LocalPlayer, gameField.gameObject)) {
            //    if (positionPrevious != position) {
            //        GetPiece().PerformMove(gameField.cells[position], this);
            //    }
            //}
            gameField.cells[position].PlacePieceLocal(this);
        }
    }
}
