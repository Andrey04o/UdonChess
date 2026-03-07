using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
using Andrey04o.Chess;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDK3.Dynamics.Constraint.Components;
namespace Andrey04o.RaycastButton {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class StationDesktopView : UdonSharpBehaviour
    {
        public VRC.SDK3.Components.VRCStation station;
        public VRC.SDK3.Components.VRCStation stationBlack;
        public GameObject desktopControl;
        public Camera camera;
        public GameField gameField;
        public GameObject blockerInteraction;
        bool isDesktopMode;
        Quaternion rotation;
        Vector3 rotationEuler;
        public Settings settings;
        bool currentSide;
        public void Enter(bool side) {
            currentSide = side;
            gameField.touchControls.ChangeMethod(false);
            rotation = camera.transform.localRotation;
            rotationEuler = rotation.eulerAngles;
            if (side) rotationEuler.y = 180;
            else rotationEuler.y = 0;
            rotation.eulerAngles = rotationEuler;
            camera.transform.localRotation = rotation;
            //station.transform.position = Networking.LocalPlayer.GetPosition();
            //station.transform.eulerAngles = new Vector3 (0f, Networking.LocalPlayer.GetRotation().eulerAngles.y, 0f);
            blockerInteraction.gameObject.SetActive(true);
            blockerInteraction.transform.position = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            Networking.LocalPlayer.Immobilize(true);
            if (!side) station.UseStation(Networking.LocalPlayer);
            else stationBlack.UseStation(Networking.LocalPlayer);
            settings.Show(true, side);
            desktopControl.SetActive(true);
            DisableInteractive = true;
            
            gameField.isTouchMode = false;
            gameField.is2DMode = true;
            gameField.ShowPieces(desktopControl.transform.rotation);
            gameField.ShowPromotion();

            camera.enabled = true;
            isDesktopMode = true;
        }
        public void Leave() {
            blockerInteraction.gameObject.SetActive(false);
            desktopControl.SetActive(false);
            Networking.LocalPlayer.Immobilize(false);
            if (!currentSide) station.ExitStation(Networking.LocalPlayer);
            else stationBlack.ExitStation(Networking.LocalPlayer);
            settings.Show(false);
            DisableInteractive = false;

            gameField.is2DMode = false;
            gameField.ShowPieces(Quaternion.identity);
            gameField.ShowPromotion();

            camera.enabled = false;
            isDesktopMode = false;
        }
        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            base.OnPlayerRespawn(player);
            if (isDesktopMode == true) {
                Leave();
            }
        }
    }
}
