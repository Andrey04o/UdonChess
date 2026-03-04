using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;
using UdonSharp;
using Andrey04o.Chess;
namespace Andrey04o.RaycastButton {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TileRaycastHandler : UdonSharpBehaviour
    {
        public Camera mainCamera;
        public CursorController cursorController;
        public LayerMask tileLayerMask = -1; // Default to all layers
        
        private RaycastButton _currentHoveredTile;
        private RaycastButton _lastHoveredTile;
        [HideInInspector] public Piece currentPiece;
        private bool isHold = false;
        
        private void Update()
        {
            HandleTileRaycast();
        }

        public override void InputUse(bool value, UdonInputEventArgs args)
        {
            if (value == true) {
                //Press();
            }
        }

        private void HandleTileRaycast()
        {
            //Networking.LocalPlayer
            //Ray ray = mainCamera.ViewportPointToRay(cursorController.currentPositionScreen);
            #if UNITY_2022_3_62
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            #else
            Ray ray = mainCamera.ScreenPointToRay(cursorController.currentPositionScreen);
            #endif
            RaycastHit hit;
            if (Input.GetMouseButtonDown(0)) {
                isHold = true;
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayerMask))
            {
                cursorController.positionReal = hit.point;
                RaycastButton tile = hit.collider.GetComponent<RaycastButton>();
                
                if (tile != null)
                {
                    
                    _currentHoveredTile = tile;

                    // Handle hover enter
                    if (_lastHoveredTile != tile)
                    {
                        // Exit previous tile
                        if (_lastHoveredTile != null)
                        {
                            _lastHoveredTile.OnRaycastExit();
                        }

                        // Enter new tile
                        tile.OnRaycastEnter();
                        _lastHoveredTile = tile;
                    }

                    // Handle click
                    if (Input.GetMouseButtonDown(0))
                    {
                        tile.OnRaycastClick(-cursorController.currentPositionScreen);
                        tile.OnRaycastDrag(this);
                    }
                    if (Input.GetMouseButtonUp(0)) {
                        tile.OnRaycastMouseUp(this);
                        currentPiece = null;
                    }
                }
                else
                {
                    // Hit something but not a tile
                    HandleNoTileHit();
                }
            }
            else
            {
                // Didn't hit anything
                HandleNoTileHit();
            }
            if (Input.GetMouseButtonUp(0)) {
                isHold = false;
                if (currentPiece != null) {
                    currentPiece.StopGrab(null);
                    currentPiece = null;
                }
            }
        }

        private void HandleNoTileHit()
        {
            if (_lastHoveredTile != null)
            {
                _lastHoveredTile.OnRaycastExit();
                _lastHoveredTile = null;
            }
            _currentHoveredTile = null;
        }
    }
}
