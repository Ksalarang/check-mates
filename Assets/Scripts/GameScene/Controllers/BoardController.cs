using System.Collections.Generic;
using System.ComponentModel;
using GameScene.Models;
using GameScene.Models.Pieces;
using UnityEngine;
using Utils.Extensions;
using Zenject;

namespace GameScene.Controllers {
public class BoardController : MonoBehaviour {
    const int BoardSize = 8;
    
    [Header("Prefabs")]
    [SerializeField] Transform boardContainer;
    [SerializeField] Transform pieceContainer;
    [SerializeField] GameObject lightSquarePrefab;
    [SerializeField] GameObject darkSquarePrefab;
    [SerializeField] GameObject piecePrefab;

    [Inject] DiContainer diContainer;
    [Inject] new Camera camera;
    [Inject] PieceSpriteProvider pieceSpriteProvider;

    readonly Square[,] squareArray = new Square[BoardSize, BoardSize];
    float squareLength;

    readonly List<Piece> whitePieces = new();
    readonly List<Piece> blackPieces = new();

    void Awake() {
        createBoard();
        createPieces();
    }

    void createBoard() {
        var cameraSize = camera.getSize();
        var boardLength = Mathf.Min(cameraSize.x, cameraSize.y);
        squareLength = boardLength / BoardSize;

        var center = new Vector3();
        var firstSquareOffset = (BoardSize / 2f) * squareLength - squareLength / 2f;
        var firstSquarePosition = new Vector3(center.x - firstSquareOffset, center.y - firstSquareOffset);

        for (var y = 0; y < BoardSize; y++) {
            for (var x = 0; x < BoardSize; x++) {
                var prefab = (y % 2 == 0 && x % 2 == 0) || (y % 2 != 0 && x % 2 != 0) 
                    ? darkSquarePrefab : lightSquarePrefab;
                var square = diContainer.InstantiatePrefabForComponent<Square>(prefab, boardContainer);
                square.transform.localScale = new Vector3(squareLength, squareLength);
                square.transform.localPosition = new Vector3(
                    firstSquarePosition.x + x * squareLength,
                    firstSquarePosition.y + y * squareLength
                );
                
                squareArray[y, x] = square;
            }
        }
    }

    void createPieces() {
        createPieceSet(true, true);
        createPieceSet(false, false);
    }

    void createPieceSet(bool white, bool bottom) {
        var list = white ? whitePieces : blackPieces;
        
        const int pawnCount = 8;
        for (var i = 0; i < pawnCount; i++) {
            var pawn = createPiece<Pawn>(white, i);
            
            var y = bottom ? 1 : BoardSize - 2;
            var x = bottom ? i : BoardSize - 1 - i;
            squareArray[y, x].tryPlacingPiece(pawn);
            
            list.Add(pawn);
        }
    }

    T createPiece<T>(bool white, int index) where T : Piece {
        var pieceObject = diContainer.InstantiatePrefab(piecePrefab, pieceContainer);
        var piece = pieceObject.AddComponent<T>();
        piece.type = getPieceType<T>();
        piece.isWhite = white;
        var color = white ? "white" : "black";
        piece.name = $"{color}_{piece.type.ToString().ToLower()}_{index}";
        piece.spriteRenderer.sprite = pieceSpriteProvider.getSprite(piece.type, white);
        piece.transform.localScale = new Vector3(squareLength, squareLength);
        return piece;
    }

    PieceType getPieceType<T>() where T : Piece {
        if (typeof(T) == typeof(Pawn)) {
            return PieceType.Pawn;
        }
        if (typeof(T) == typeof(Knight)) {
            return PieceType.Knight;
        }
        if (typeof(T) == typeof(Bishop)) {
            return PieceType.Bishop;
        }
        if (typeof(T) == typeof(Rook)) {
            return PieceType.Rook;
        }
        if (typeof(T) == typeof(Queen)) {
            return PieceType.Queen;
        }
        if (typeof(T) == typeof(King)) {
            return PieceType.King;
        }
        throw new InvalidEnumArgumentException($"type {nameof(T)} is not recognized");
    }
}
}