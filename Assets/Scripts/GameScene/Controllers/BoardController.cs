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

    // readonly List<Piece> whitePieces = new();
    // readonly List<Piece> blackPieces = new();

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
        const int pawnCount = 8;
        for (var i = 0; i < pawnCount; i++) {
            createPawn(white, bottom, i);
        }

        createPairedPieces<Rook>(white, bottom, Vector2Int.zero);
        createPairedPieces<Knight>(white, bottom, new Vector2Int(1, 0));
        createPairedPieces<Bishop>(white, bottom, new Vector2Int(2, 0));
        
        createQueen(white, bottom);
        createKing(white, bottom);
    }

    T createPiece<T>(bool white, int index = -1) where T : Piece {
        var pieceObject = diContainer.InstantiatePrefab(piecePrefab, pieceContainer);
        var piece = pieceObject.AddComponent<T>();
        piece.type = getPieceType<T>();
        piece.isWhite = white;
        piece.spriteRenderer.sprite = pieceSpriteProvider.getSprite(piece.type, white);
        piece.transform.localScale = new Vector3(squareLength, squareLength);
        piece.name = getPieceName(piece, index);
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

    string getPieceName(Piece piece, int index) {
        var color = piece.isWhite ? "white" : "black";
        var pieceName = $"{color}_{piece.type.ToString().ToLower()}";
        if (index > -1) {
            pieceName += $"_{index}";
        }
        return pieceName;
    }

    Square getSquare(Vector2Int position) => getSquare(position.x, position.y);
    
    Square getSquare(int x, int y) => squareArray[y, x];

    Vector2Int getOppositePosition(Vector2Int position) {
        position.x = getOppositeIndex(position.x);
        position.y = getOppositeIndex(position.y);
        return position;
    }

    int getOppositeIndex(int index) => BoardSize - 1 - index;

    void createPawn(bool white, bool bottom, int index) {
        var pawn = createPiece<Pawn>(white, index);

        var x = bottom ? index : getOppositeIndex(index);
        var y = bottom ? 1 : getOppositeIndex(1);
        getSquare(x, y).tryPlacingPiece(pawn);
    }

    void createPairedPieces<T>(bool white, bool bottom, Vector2Int position) where T : Piece {
        var left = createPiece<T>(white, 0);
        var square = getSquare(bottom ? position : getOppositePosition(position));
        square.tryPlacingPiece(left);
        
        var right = createPiece<T>(white, 1);
        position.x = getOppositeIndex(position.x);
        square = getSquare(bottom ? position : getOppositePosition(position));
        square.tryPlacingPiece(right);
    }

    void createQueen(bool white, bool bottom) {
        var queen = createPiece<Queen>(white);
        var position = Vector2Int.zero;
        if (getSquare(3, bottom ? 0 : getOppositeIndex(0)).isLight) {
            position.x = white ? 3 : 4;
        } else {
            position.x = white ? 4 : 3;
        }
        if (!bottom) {
            position.y = getOppositeIndex(0);
        }
        var square = getSquare(position);
        square.tryPlacingPiece(queen);
    }
    
    void createKing(bool white, bool bottom) {
        var king = createPiece<King>(white);
        var position = Vector2Int.zero;
        if (getSquare(3, bottom ? 0 : getOppositeIndex(0)).isLight) {
            position.x = white ? 4 : 3;
        } else {
            position.x = white ? 3 : 4;
        }
        if (!bottom) {
            position.y = getOppositeIndex(0);
        }
        var square = getSquare(position);
        square.tryPlacingPiece(king);
    }
}
}