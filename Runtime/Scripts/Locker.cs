using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
using TMPro;
namespace Andrey04o.Chess {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Locker : UdonSharpBehaviour
    {
        public GameField gameField;
        public bool isBlack = false;
        //public Piece[] pieces;
        public TextMeshProUGUI textMeshPlayer;
        [UdonSynced] bool isLocked = false;
        bool _isLocked = false;
        public TextMeshProUGUI textMeshLock;
        
        public void ShowPlayerLocker() {
            if (isLocked) {
                textMeshPlayer.gameObject.SetActive(true);
                textMeshPlayer.text = "Locked by " + Networking.GetOwner(gameObject).displayName;
            } else {
                textMeshPlayer.gameObject.SetActive(false);
            }
        }
        public void Use() {
            if (Networking.LocalPlayer.IsOwner(gameObject) == false) {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            } else {
                isLocked = !isLocked;
                PerfomLock();
                RequestSerialization();
            }
        }
        public void PerfomLock() {
            _isLocked = isLocked;
            if (Networking.LocalPlayer.IsOwner(gameObject)) {
                _isLocked = false;
            }
            foreach (Piece piece in gameField.pieces.InTableAll) {
                if (piece.isBlack != isBlack) continue;
                piece.EnablePickup(!_isLocked);
            }
            if (isLocked) textMeshLock.text = "Unlock";
            else textMeshLock.text = "Lock";
            ShowPlayerLocker();
        }

        public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
        {
            VRCPlayerApi owner = Networking.GetOwner(gameObject);
            float ownerLength = Vector3.Distance(owner.GetPosition(), transform.position);
            Debug.Log("owner length " + ownerLength + " < " + transform.lossyScale.x);
            if (ownerLength < transform.lossyScale.x) return false;
            float reqLength = Vector3.Distance(requestedOwner.GetPosition(), transform.position);
            Debug.Log("owner length " + ownerLength + " < reqLength" + reqLength);
            return reqLength < ownerLength;
        }
        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            base.OnOwnershipTransferred(player);
            if (player == Networking.LocalPlayer) {
                isLocked = false;
                PerfomLock();
                RequestSerialization();
            }
            ShowPlayerLocker();
        }
        public override void OnDeserialization()
        {
            base.OnDeserialization();
            PerfomLock();
        }
        
    }
}