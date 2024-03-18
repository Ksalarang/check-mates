using System;
using GameScene.Models.BoardModel;
using GameScene.Models.Pieces;
using UnityEngine;

namespace GameScene.Models.SessionModel {
public class Turn {
    public readonly int number;
    public Type type;
    public Piece piece;
    public Square startSquare;
    public Square endSquare;
    public Piece capturedPiece;

    public Turn(int number) {
        this.number = number;
        type = Type.NotDetermined;
    }

    public void determineType() {
        if (type is not Type.NotDetermined) return;
        
        if (capturedPiece is not null) {
            type = Type.Capture;
        } else {
            type = Type.Move;
        }
    }

    public bool isEnPassant() {
        return piece.type == PieceType.Pawn && Mathf.Abs(startSquare.indices.y - endSquare.indices.y) == 2;
    }

    public override string ToString() {
        var result = type switch {
            Type.Move => $"{piece} moves: {startSquare} -> {endSquare}",
            Type.Capture => $"{piece} captures {capturedPiece}, {startSquare} -> {endSquare}",
            Type.EnPassantCapture => $"{piece} captures en passant {capturedPiece}, {startSquare} -> {endSquare}",
            _ => throw new ArgumentOutOfRangeException()
        };
        return $"turn {number}: {result}";
    }
    
    public enum Type {
        NotDetermined,
        Move,
        Capture,
        EnPassantCapture,
    }
}
}