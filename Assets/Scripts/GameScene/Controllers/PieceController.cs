using System;
using System.Collections.Generic;
using System.ComponentModel;
using GameScene.Models.Pieces;
using UnityEngine;
using Zenject;

namespace GameScene.Controllers {
public class PieceController : MonoBehaviour {
    [Header("Prefabs")]
    [SerializeField] Transform pieceContainer;
    [SerializeField] GameObject piecePrefab;

    [Inject] DiContainer diContainer;
    [Inject] PieceSpriteProvider pieceSpriteProvider;
    [Inject] BoardController boardController;

    public List<Piece> whitePieces;
    public List<Piece> blackPieces;

    void Awake() {
        whitePieces = new List<Piece>();
        blackPieces = new List<Piece>();
    }

    public void createPieces() {
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
        var piece = diContainer.InstantiateComponent<T>(pieceObject);
        piece.type = getPieceType<T>();
        piece.isWhite = white;
        piece.spriteRenderer.sprite = pieceSpriteProvider.getSprite(piece.type, white);
        var scale = boardController.getSquareLength();
        piece.transform.localScale = new Vector3(scale, scale);
        piece.index = index;
        piece.name = piece.ToString();

        var list = white ? whitePieces : blackPieces;
        list.Add(piece);
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

    Vector2Int getOppositePosition(Vector2Int position) {
        position.x = getOppositeIndex(position.x);
        position.y = getOppositeIndex(position.y);
        return position;
    }

    int getOppositeIndex(int index) => BoardController.BoardSize - 1 - index;

    void createPawn(bool white, bool bottom, int index) {
        var pawn = createPiece<Pawn>(white, index);

        var x = bottom ? index : getOppositeIndex(index);
        var y = bottom ? 1 : getOppositeIndex(1);
        boardController.getSquare(x, y).tryPlacingPiece(pawn);
    }

    void createPairedPieces<T>(bool white, bool bottom, Vector2Int position) where T : Piece {
        var left = createPiece<T>(white, 0);
        var square = boardController.getSquare(bottom ? position : getOppositePosition(position));
        square.tryPlacingPiece(left);
        
        var right = createPiece<T>(white, 1);
        position.x = getOppositeIndex(position.x);
        square = boardController.getSquare(bottom ? position : getOppositePosition(position));
        square.tryPlacingPiece(right);
    }

    void createQueen(bool white, bool bottom) {
        var queen = createPiece<Queen>(white);
        var position = Vector2Int.zero;
        if (boardController.getSquare(3, bottom ? 0 : getOppositeIndex(0)).isLight) {
            position.x = white ? 3 : 4;
        } else {
            position.x = white ? 4 : 3;
        }
        if (!bottom) {
            position.y = getOppositeIndex(0);
        }
        var square = boardController.getSquare(position);
        square.tryPlacingPiece(queen);
    }
    
    void createKing(bool white, bool bottom) {
        var king = createPiece<King>(white);
        var position = Vector2Int.zero;
        if (boardController.getSquare(3, bottom ? 0 : getOppositeIndex(0)).isLight) {
            position.x = white ? 4 : 3;
        } else {
            position.x = white ? 3 : 4;
        }
        if (!bottom) {
            position.y = getOppositeIndex(0);
        }
        var square = boardController.getSquare(position);
        square.tryPlacingPiece(king);
    }
}
}