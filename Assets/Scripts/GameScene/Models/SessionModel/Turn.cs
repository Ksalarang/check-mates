using System;
using GameScene.Models.BoardModel;
using GameScene.Models.Pieces;

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
    }

    public void determineType() {
        if (capturedPiece is not null) {
            type = Type.Capture;
        } else {
            type = Type.Move;
        }
    }

    public override string ToString() {
        var result = type switch {
            Type.Move => $"{piece} moves: {startSquare} -> {endSquare}",
            Type.Capture => $"{piece} captures {capturedPiece}, {startSquare} -> {endSquare}",
            _ => throw new ArgumentOutOfRangeException()
        };
        return $"turn {number}: {result}";
    }
    
    public enum Type {
        Move,
        Capture,
    }
}
}