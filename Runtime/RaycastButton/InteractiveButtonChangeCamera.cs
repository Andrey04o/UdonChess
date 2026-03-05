using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
using Andrey04o.Chess;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VRC.SDK3.Components;
namespace Andrey04o.RaycastButton {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class InteractiveButtonChangeCamera : UdonSharpBehaviour
    {
        public VRC.SDK3.Components.VRCStation station;
        public GameObject desktopControl;
        public Camera camera;
        public GameField gameField;
        public override void Interact()
        {
            base.Interact();
            station.UseStation(Networking.LocalPlayer);
            desktopControl.SetActive(true);
            DisableInteractive = true;
            gameField.Set2DView(true, desktopControl.transform.rotation);
            camera.enabled = true;
        }

        public void Leave() {
            desktopControl.SetActive(false);
            station.ExitStation(Networking.LocalPlayer);
            DisableInteractive = false;
            gameField.Set2DView(false);
            camera.enabled = false;
        }

        public void TestInterr() {
            Debug.Log("DRAGG WTF");
            //Debug.Log("WTF " + baseEventData.currentInputModule.input.compositionCursorPos);
        }
        
        public void TestSelect() {
            Debug.Log("SELECT WTF");
        }
        public void VectorTest() {
            Debug.Log("vector ");
        }
        public void TestBegindrag() {
            Debug.Log("BEGINDRAG ");
        }
        public void TestEnddrag() {
            Debug.Log("ENDDRAG ");
        }
        public void TestPointerdown() {
            Debug.Log("pointerdown ");
        }
    }
}
