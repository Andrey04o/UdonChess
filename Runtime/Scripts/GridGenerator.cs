using UnityEngine;
using System.Collections.Generic;
using VRC;
using Andrey04o.RaycastButton;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Andrey04o.Chess {
    public class GridGenerator : MonoBehaviour
    {
        public Pieces pieces;
        public Piece piecePrefab;
        public Pieces piecesPrefab;
        [SerializeField] private Cell cellPrefab;
        [SerializeField] private Line linePrefab;
        [SerializeField] private GameField fieldPrefab;
        [SerializeField] private int gridWidth = 10;
        [SerializeField] private int gridHeight = 20;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private float padding = 0.1f;
        List<Piece> piecesList = new List<Piece>();
        List<Piece> kingsList = new List<Piece>();
        public InteractiveButtonChangeCamera interactiveButtonChangeCamera;
        public Material materialBlack;
        public Material materialWhite;
        
        private Transform gridContainer;
        #if UNITY_EDITOR
        public void GenerateGrid()
        {
            // Clear existing grid if any
            if (gridContainer != null)
            {
                DestroyImmediate(gridContainer.gameObject);
            }

            // Create container for grid cells
            GameField gameField = PrefabUtility.InstantiatePrefab(fieldPrefab, this.transform) as GameField;
            
            gameField.transform.SetParent(transform);
            gameField.transform.localPosition = Vector3.zero;

            Pieces pieces = PrefabUtility.InstantiatePrefab(piecesPrefab, this.transform) as Pieces;
            pieces.transform.SetParent(gameField.transform);
            pieces.gameObject.SetActive(false);
            gameField.pieces = pieces;
            interactiveButtonChangeCamera.gameField = gameField;
            Vector3 cellPosition = new Vector3(0, 0, 0);
            List<Cell> cells = new List<Cell>();
            List<Cell> cells2 = new List<Cell>();
            List<Line> lines = new List<Line>();
            // Generate grid cells
            for (int y = 0; y < gridHeight; y++)
            {
                Line line = PrefabUtility.InstantiatePrefab(linePrefab, gameField.transform) as Line;
                float yPos = -y * (cellSize + padding);
                cellPosition = new Vector3(0, 0, yPos);
                line.transform.localPosition = cellPosition;
                line.name = $"Line_{y}";
                for (int x = 0; x < gridWidth; x++)
                {
                    // Calculate position with padding
                    float xPos = x * (cellSize + padding);

                    cellPosition = new Vector3(xPos, 0, 0);

                    // Instantiate prefab using PrefabUtility
                    Cell cell = PrefabUtility.InstantiatePrefab(cellPrefab, line.transform) as Cell;
                    
                    if (cell != null)
                    {
                        if ((cells2.Count + lines.Count) % 2 == 0) {
                            cell.meshRenderer.sharedMaterial = materialBlack;
                            cell.materialNormal = materialBlack;
                            cell.materialCurrent = materialBlack;
                            cell.materialAttackColored = materialBlack;
                        } else {
                            cell.meshRenderer.sharedMaterial = materialWhite;
                            cell.materialNormal = materialWhite;
                            cell.materialCurrent = materialWhite;
                            cell.materialAttackColored = materialWhite;
                        }
                        cell.transform.localPosition = cellPosition;
                        cell.name = $"Cell_{x}_{y}";
                        cell.line = line;
                        cell.index = (byte)cells.Count;
                        cells.Add(cell);
                        cell.position = (byte)cells2.Count;
                        cells2.Add(cell); 
                        cell.gameField = gameField;
                    }
                }
                line.cells = cells.ToArray();
                line.gameField = gameField;
                EditorUtility.SetDirty(line);
                line.index = (byte)lines.Count;
                lines.Add(line);
                cells.Clear();
            }
            gameField.lines = lines.ToArray();
            gameField.cells = cells2.ToArray();
            EditorUtility.SetDirty(gameField);
            lines.Clear();
            
            piecesList.Clear();
            // Place chess pieces
            PlaceChessPieces(gameField);
            
            pieces.InTableAll = piecesList.ToArray();
            pieces.allKings = kingsList.ToArray();
            Debug.Log(pieces.InTableAll.Length);
            Debug.Log(piecesList.Count);
            gameField.promotionPiece = byte.MaxValue;
            EditorUtility.SetDirty(pieces);
            gameField.CalcAttacks();
            foreach(Cell cell in cells2) {
                EditorUtility.SetDirty(cell);
                
            }
            cells2.Clear();
            EditorUtility.SetDirty(interactiveButtonChangeCamera);
        }
        byte idChess = 0;
        private void PlaceChessPieces(GameField gameField)
        {
            if (pieces == null)
            {
                Debug.LogWarning("Pieces reference is not set!");
                return;
            }
            idChess = 0;
            // Place white pieces (bottom rows, y = 6 and 7 for 8x8 grid)
            PlacePiecesForPlayer(gameField, 6, 7, pieces.materialWhite, false);
            
            // Place black pieces (top rows, y = 0 and 1 for 8x8 grid)
            PlacePiecesForPlayer(gameField, 1, 0, pieces.materialBlack, true);
        }
        
        private void PlacePiecesForPlayer(GameField gameField, int pawnRow, int backRow, Material material, bool isBlack)
        {
            if (gameField.lines.Length < backRow + 1) return;
            Line pawnLine = gameField.lines[pawnRow];
            Line backLine = gameField.lines[backRow];
            
            // Place pawns
            for (int x = 0; x < 8 && x < pawnLine.cells.Length; x++)
            {
                PlacePiece(gameField, pieces.pawn, pawnLine.cells[x], material, isBlack);
            }
            
            // Place back row pieces
            if (backLine.cells.Length >= 8)
            {
                PlacePiece(gameField, pieces.rook, backLine.cells[0], material, isBlack);
                PlacePiece(gameField, pieces.knight, backLine.cells[1], material, isBlack);
                PlacePiece(gameField, pieces.bishop, backLine.cells[2], material, isBlack);
                PlacePiece(gameField, pieces.queen, backLine.cells[3], material, isBlack);
                PlacePiece(gameField, pieces.king, backLine.cells[4], material, isBlack);
                PlacePiece(gameField, pieces.bishop, backLine.cells[5], material, isBlack);
                PlacePiece(gameField, pieces.knight, backLine.cells[6], material, isBlack);
                PlacePiece(gameField, pieces.rook, backLine.cells[7], material, isBlack);
            }
        }
        
        private void PlacePiece(GameField gameField,Piece piecePrefab, Cell cell, Material material, bool isBlack)
        {
            if (piecePrefab == null || cell == null) return;
            //PrefabUtility.GetPrefabObject(piecePrefab)
            Piece piece = PrefabUtility.InstantiatePrefab(this.piecePrefab, cell.transform) as Piece;
            piece.gameField = gameField;
            //if (piece == null) piece = PrefabUtility.InstantiatePrefab(PrefabUtility.GetOriginalSourceRootWhereGameObjectIsAdded(piecePrefab.gameObject), cell.transform) as Piece;
            if (piece != null)
            {
                piece.isBlack = isBlack;
                if (isBlack) {
                    piece.forward = new Vector2Int(0,1);
                    piece.left = new Vector2Int(1,0);
                    piece.promotion.rotation.eulerAngles = new Vector3(0, 180, 0); 
                } else {
                    piece.forward = new Vector2Int(0,-1);
                    piece.left = new Vector2Int(-1,0);
                    piece.promotion.rotation.eulerAngles = new Vector3(0, 0, 0); 
                }
                piece.promotion.transform.localRotation = piece.promotion.rotation;
                piece.rotationConstraint.Sources.Add(new VRC.Dynamics.VRCConstraintSource(interactiveButtonChangeCamera.desktopControl.transform, 1f));
                piece.promotion.rotationConstraint.Sources.Add(new VRC.Dynamics.VRCConstraintSource(interactiveButtonChangeCamera.desktopControl.transform, 1f));
                piece.transform.position = cell.positionPiece.transform.position;
                cell.pieceCurrent = piece;
                // Apply material to the visible model
                piece.meshRenderer.material = material;
                piece.ChangeType(piecePrefab.indexType);
                piece.position = cell.position;
                piece.id = idChess;
                idChess++;
                piecesList.Add(piece);
                piece.positionPrevious = piece.position;
                cell.PlacePiece(piece);
                piece.isNotMoved = 0;
                if (pieces.king == piecePrefab) {
                    kingsList.Add(piece);
                }
                EditorUtility.SetDirty(cell);
                EditorUtility.SetDirty(piece);
                EditorUtility.SetDirty(piece.promotion);
            }
        }
        #endif

        public void ClearGrid()
        {
            if (gridContainer != null)
            {
                DestroyImmediate(gridContainer.gameObject);
                gridContainer = null;
            }
        }
    }
}