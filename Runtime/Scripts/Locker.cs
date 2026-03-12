using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
using TMPro;
using UnityEngine.UI;
using Andrey04o.Chess.RaycastButton;
namespace Andrey04o.Chess {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Locker : UdonSharpBehaviour
    {
        public GameField gameField;
        public bool isBlack = false;
        //public Piece[] pieces;
        public TextMeshProUGUI textMeshPlayer;
        public Image imageLock;
        public Sprite spriteLock;
        public Sprite spriteUnlock;
        [UdonSynced] bool isLocked = false;
        bool _isLocked = false;
        bool isInZone = false;
        public DesktopControls desktopControls;
        
        public void ShowPlayerLocker() {
            if (isLocked) {
                imageLock.sprite = spriteLock;
                textMeshPlayer.gameObject.SetActive(true);
                textMeshPlayer.text = "Locked by " + Networking.GetOwner(gameObject).displayName;
            } else {
                imageLock.sprite = spriteUnlock;
                textMeshPlayer.gameObject.SetActive(false);
            }
            if (desktopControls != null) {
                ShowCurrent(ref desktopControls.textLock, ref desktopControls.imageLock);
            }
        }
        public void Use() {
            if (isInZone == false) return;
            if (Networking.LocalPlayer.IsOwner(gameObject) == false) {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            } else {
                isLocked = !isLocked;
                PerfomLock();
                RequestSerialization();
            }
        }
        public void Lock() {
            if (isInZone == false) return;
            if (isLocked == false) {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                isLocked = true;
                PerfomLock();
                RequestSerialization();
            }
        }
        public bool IsCanUse() {
            return !_isLocked;
        }
        public void PerfomLock() {
            _isLocked = isLocked;
            if (Networking.LocalPlayer.IsOwner(gameObject)) {
                _isLocked = false;
            }
            foreach (Piece piece in gameField.pieces.InTableAll) {
                if (piece.isBlack != isBlack) continue;
                piece.EnablePickup(!_isLocked);
                //piece.StopGrab(null);
            }
            ShowPlayerLocker();
        }

        public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
        {
            if (isLocked == false) return true;
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
        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            base.OnPlayerLeft(player);
            PerformDisableLock(player);
        }
        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            base.OnPlayerRespawn(player);
            PerformDisableLock(player);
        }
        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            base.OnPlayerTriggerExit(player);
            PerformDisableLock(player);
        }
        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            base.OnPlayerTriggerEnter(player);
            if (player.isLocal) {
                isInZone = true;
            }
        }
        public void PerformDisableLock(VRCPlayerApi player) {
            if (isLocked == false) return;
            if (player.IsOwner(gameObject)) {
                isLocked = false;
                isInZone = false;
                ShowPlayerLocker();
                RequestSerialization();
            }
        }
        public void ShowCurrent(ref TextMeshProUGUI text, ref Image image) {
            text.gameObject.SetActive(textMeshPlayer.gameObject.activeSelf);
            text.text = textMeshPlayer.text;
            image.sprite = imageLock.sprite;
        }
        public override void OnDeserialization()
        {
            base.OnDeserialization();
            PerfomLock();
        }
        
    }
}