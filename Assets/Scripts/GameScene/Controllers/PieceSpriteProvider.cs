using System;
using GameScene.Models.Pieces;
using UnityEngine;

namespace GameScene.Controllers {
public class PieceSpriteProvider : MonoBehaviour {
    [Header("White")]
    [SerializeField] Sprite whitePawn;
    [SerializeField] Sprite whiteKnight;
    [SerializeField] Sprite whiteBishop;
    [SerializeField] Sprite whiteRook;
    [SerializeField] Sprite whiteQueen;
    [SerializeField] Sprite whiteKing;
    [Header("Black")]
    [SerializeField] Sprite blackPawn;
    [SerializeField] Sprite blackKnight;
    [SerializeField] Sprite blackBishop;
    [SerializeField] Sprite blackRook;
    [SerializeField] Sprite blackQueen;
    [SerializeField] Sprite blackKing;

    public Sprite getSprite(PieceType type, bool white) => white ? getWhitePiece(type) : getBlackPiece(type);

    Sprite getWhitePiece(PieceType type) {
        return type switch {
            PieceType.Pawn => whitePawn,
            PieceType.Knight => whiteKnight,
            PieceType.Bishop => whiteBishop,
            PieceType.Rook => whiteRook,
            PieceType.Queen => whiteQueen,
            PieceType.King => whiteKing,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
    
    Sprite getBlackPiece(PieceType type) {
        return type switch {
            PieceType.Pawn => blackPawn,
            PieceType.Knight => blackKnight,
            PieceType.Bishop => blackBishop,
            PieceType.Rook => blackRook,
            PieceType.Queen => blackQueen,
            PieceType.King => blackKing,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
}