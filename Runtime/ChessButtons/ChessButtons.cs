
using Andrey04o.RaycastButton;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
namespace Andrey04o.Chess {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ChessButtons : UdonSharpBehaviour
    {
        public GameField gameField;
        public Animator animatorRestart;
        public Animator animatorResync;
        public TextMeshProUGUI textRestart;
        public TextMeshProUGUI textUpdate;
        public TextMeshProUGUI textMeshTouchScreen;
        public TouchControls touchControls;
        public StationDesktopView stationDesktopView;
        public Station[] stations;
        public GameObject aboutPage;
        public Locker lockerBlack;
        public Locker lockerWhite;
        public void Press() {
            if (gameField.isStalemate > 0) {
                Restart();
                return;
            }
            if (textRestart.gameObject.activeSelf == false) {
                animatorRestart.Play("RestartButton", 0, 0f);
            } else {
                Restart();
            }
        }

        public void Restart() {
            NetworkCalling.SendCustomNetworkEvent((IUdonEventReceiver)gameField, NetworkEventTarget.Owner, nameof(GameField.RestartBoardRemote));
        }

        public void Resync() {
            NetworkCalling.SendCustomNetworkEvent((IUdonEventReceiver)gameField, NetworkEventTarget.Owner, nameof(GameField.AskAboutUpdate));
            if (textRestart.gameObject.activeSelf == false) {
                animatorResync.Play("RestartButton", 0, 0f);
            }
        }
        public void ToggleTouchScreen() {
            touchControls.ChangeMethod(!touchControls.isEnabledTouchSupport);
        }
        public void DesktopModeWhite() {
            stationDesktopView.Enter(false);
        }
        public void DesktopModeBlack() {
            stationDesktopView.Enter(true);
        }
        public void LockerWhite() {
            lockerWhite.Use();
        }
        public void LockerBlack() {
            lockerBlack.Use();
        }
        public void Show(bool value) {
            gameObject.SetActive(value);
            #if UNITY_STANDALONE
            if (value == true) {
                if (Networking.LocalPlayer.IsUserInVR() == false) {
                    foreach(Station station in stations) {
                        station.ShowButton();
                    }
                }
            }
            #endif
        }
        public void ToggleAbout() {
            aboutPage.SetActive(!aboutPage.activeSelf);
        }
    }
}