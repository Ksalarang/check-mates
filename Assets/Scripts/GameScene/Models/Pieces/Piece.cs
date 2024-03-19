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
        awake();
    }

    protected virtual void awake() {}

    protected bool isPathClear(Vector2Int direction, int steps) {
        for (var i = 1; i <= steps; i++) {
            var nextSquare = board.getSquare(position + direction * i);
            if (nextSquare is null || nextSquare.hasPiece()) return false;
        }
        return true;
    }

    protected List<Square> getAvailableSquaresInDirections(List<PieceDirection> directions) {
        var list = new List<Square>();
        foreach (var direction in directions) {
            var square = getSquareInDirection(direction, 1);
            for (var step = 2; square is not null; square = getSquareInDirection(direction, step++)) {
                if (!square.hasPiece()) {
                    list.Add(square);
                } else {
                    if (isSameSide(square.currentPiece)) {
                        break;
                    }
                    if (square.currentPiece.type != PieceType.King) {
                        list.Add(square);
                    }
                    break;
                }
            }
        }
        return list;
    }

    public int getRelativeIndex(int i) {
        return isBottom ? i : Board.getOppositeIndex(i);
    }

    public abstract List<Square> getAvailableSquares();

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

    public bool isSameSide(Piece other) => isWhite == other.isWhite;

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