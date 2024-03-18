using System;
using System.Collections.Generic;
using GameScene.Models.BoardModel;
using GameScene.Models.SessionModel;
using UnityEngine;
using Zenject;

namespace GameScene.Models.Pieces {
public abstract class Piece : MonoBehaviour {
    public PieceType type;
    public bool isWhite;
    public bool isBottom;
    public int index;
    public Square currentSquare;

    [HideInInspector] public SpriteRenderer spriteRenderer;

    public Vector2Int position => currentSquare.indices;

    [Inject] protected Board board;
    [Inject] protected Session session;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public abstract List<Square> getAvailableSquares();

    protected bool isPathClear(Vector2Int direction, int steps) {
        for (var i = 1; i <= steps; i++) {
            var nextSquare = board.getSquare(position + direction * i);
            if (nextSquare is null || nextSquare.hasPiece()) return false;
        }
        return true;
    }

    public Square getSquareInDirection(PieceDirection direction, int steps = 1) {
        return board.getSquare(position + getRelativeDirection(direction) * steps);
    }

    public Vector2Int getRelativeDirection(PieceDirection relative) {
        var absolute = relative switch {
            PieceDirection.Forward => BoardDirection.Up,
            PieceDirection.ForwardRight => BoardDirection.UpRight,
            PieceDirection.Right => BoardDirection.Right,
            PieceDirection.BackwardRight => BoardDirection.DownRight,
            PieceDirection.Backward => BoardDirection.Down,
            PieceDirection.BackwardLeft => BoardDirection.DownLeft,
            PieceDirection.Left => BoardDirection.Left,
            PieceDirection.ForwardLeft => BoardDirection.UpLeft,
            _ => throw new ArgumentOutOfRangeException(nameof(relative), relative, null)
        };
        return isBottom ? absolute : -absolute;
    }
    
    protected int getRelativeIndex(int i) {
        return isBottom ? i : Board.getOppositeIndex(i);
    }

    public override string ToString() {
        var color = isWhite ? "w" : "b";
        var result = $"{color}_{type.ToString().ToLower()}";
        if (index > -1) {
            result += $"_{index}";
        }
        return result;
    }
}

public enum PieceType {
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}
}