using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
namespace Andrey04o.RaycastButton {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Station : UdonSharpBehaviour
    {
        public GameObject buttonInteractive;
        public bool isInUse;
        public override void OnStationEntered(VRCPlayerApi player)  
        {  
            base.OnStationEntered(player);
            #if UNITY_STANDALONE
                if (Networking.LocalPlayer.IsUserInVR() == false) {
                    buttonInteractive.SetActive(false);
                    isInUse = true;
                }
            #endif
        }
        public override void OnStationExited(VRCPlayerApi player)
        {
            base.OnStationExited(player);
            #if UNITY_STANDALONE
                if (Networking.LocalPlayer.IsUserInVR() == false) {
                    buttonInteractive.SetActive(true);
                    isInUse = false;
                }
            #endif
        }
        public void ShowButton(bool value = true) {
            if (value == false) {
                buttonInteractive.SetActive(false);
            } else {
                buttonInteractive.SetActive(!isInUse);
            }
            
        }

    }
}
