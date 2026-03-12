using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
using TMPro;
using Andrey04o.Chess.RaycastButton;
namespace Andrey04o.Chess {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class OwnerManager : UdonSharpBehaviour
    {
        public GameField gameField;
        public ChessButtons chessButtons;
        public TouchControls touchControls;
        public bool imIn = false;
        public VRCPlayerApi currentOwner;
        public TextMeshPro text1;
        public GameObject visual;
        public void BecomeOwner() {
            Networking.SetOwner(Networking.LocalPlayer, gameField.gameObject);
        }
        public void SetButtons(bool value) {
            chessButtons.Show(value);
        }
        public void SetTouchControls(bool value) {
            touchControls.gameObject.SetActive(value);
            if (value == true) {
                touchControls.ChangeMethod(VRC.SDKBase.InputManager.GetLastUsedInputMethod());
            }
            
        }
        public void SetPiecesCollision(bool value) {
            foreach (Piece piece in gameField.pieces.InTableAll) {
                if (piece.isCaptured == 0) {
                    piece.ShowPiece(Quaternion.identity);
                }
            }
        }
        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (player.isLocal) {
                imIn = true;
                SetButtons(true);
                SetTouchControls(true);
                SetPiecesCollision(true);
                visual.gameObject.SetActive(true);
                BecomeOwner();
            }
            base.OnPlayerTriggerEnter(player);
        }
        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            base.OnPlayerTriggerExit(player);
            if (player.isLocal) {
                imIn = false;
                SetButtons(false);
                SetTouchControls(false);
                SetPiecesCollision(false);
                visual.gameObject.SetActive(false);
            }
        }
        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            base.OnPlayerLeft(player);
            if (imIn == false) return;
            if (currentOwner == null || player == currentOwner) {
                BecomeOwner();
            }
        }
        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            base.OnPlayerRespawn(player);
            if (player.isLocal) {
                imIn = false;
                SetButtons(false);
                SetTouchControls(false);
                SetPiecesCollision(false);
                visual.gameObject.SetActive(false);
            }
        }
    }

}