
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
namespace Andrey04o.Chess {
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class RestartButton : UdonSharpBehaviour
{
    public GameField gameField;
    public Button button;
    public Animator animatorRestart;
    public TextMeshProUGUI textRestart;

    public void Press() {
        if (gameField.isStalemate == 2) {
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
        Networking.SetOwner(Networking.LocalPlayer, gameField.gameObject);
        gameField.RestartBoard();
    }

    public void Resync() {
        Networking.SetOwner(Networking.LocalPlayer, gameField.gameObject);
        gameField.RequestSerialization();
    }
}
}