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

    #region Awake
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        awake();
    }

    protected virtual void awake() {}
    #endregion
    
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

    #region Absolute pin
    protected void checkAbsolutePin(List<Square> availableSquares) {
        if (isAbsolutePinned(out var pinningPiece)) {
            if (getDirectionTo(pinningPiece, out var dirVector)) {
                var list = new List<Square>();
                for (var step = 1; step <= Board.Size; step++) {
                    var nextSquare = getSquareInDirection(vectorToDirection(dirVector), step);
                    list.Add(nextSquare);
                    if (nextSquare.currentPiece == pinningPiece) break;
                }
                dirVector = -dirVector;
                var king = session.getKing(isWhite);
                for (var step = 1; step <= Board.Size; step++) {
                    var nextSquare = getSquareInDirection(vectorToDirection(dirVector), step);
                    if (nextSquare.currentPiece == king) break;
                    list.Add(nextSquare);
                }
                availableSquares.RemoveAll(s => !list.Contains(s));
            }
        }
    }

    bool isAbsolutePinned(out Piece pinningPiece) {
        var king = session.getKing(isWhite);
        if (hasClearPathTo(king)) {
            if (getDirectionTo(king, out var dirVector)) {
                dirVector = -dirVector;
                if (isOpponentPieceOnPath(vectorToDirection(dirVector), out var enemyPiece)) {
                    if (BoardDirection.isDiagonal(dirVector)) {
                        if (enemyPiece.type is PieceType.Queen or PieceType.Bishop) {
                            pinningPiece = enemyPiece;
                            return true;
                        }
                        pinningPiece = null;
                        return false;
                    }
                    if (enemyPiece.type is PieceType.Queen or PieceType.Rook) {
                        pinningPiece = enemyPiece;
                        return true;
                    }
                    pinningPiece = null;
                    return false;
                }
            }
        }
        pinningPiece = null;
        return false;
    }
    
    bool isOpponentPieceOnPath(PieceDirection direction, out Piece opponent) {
        for (var step = 1; step <= Board.Size; step++) {
            var nextSquare = getSquareInDirection(direction, step);
            if (nextSquare is null) {
                opponent = null;
                return false;
            }
            if (nextSquare.hasPiece()) {
                if (isSameSide(nextSquare.currentPiece)) {
                    opponent = null;
                    return false;
                }
                if (nextSquare.currentPiece.type is PieceType.Bishop or PieceType.Rook or PieceType.Queen) {
                    opponent = nextSquare.currentPiece;
                    return true;
                }
                opponent = null;
                return false;
            }
        }
        opponent = null;
        return false;
    }
    #endregion

    #region Interface
    public abstract List<Square> getAvailableSquares();
    
    public int getRelativeIndex(int i) {
        return isBottom ? i : Board.getOppositeIndex(i);
    }

    public Square getSquareInDirection(PieceDirection direction, int steps = 1) {
        return board.getSquare(position + getRelativeDirection(direction) * steps);
    }

    public override string ToString() {
        var color = isWhite ? "w" : "b";
        var result = $"{color}_{type.ToString().ToLower()}";
        if (index > -1) {
            result += $"_{index}";
        }
        return result;
    }
    #endregion

    #region Helpers
    bool hasClearPathTo(Piece piece) {
        if (getDirectionTo(piece, out var direction)) {
            for (var step = 1; step <= Board.Size; step++) {
                var nextSquare = getSquareInDirection(vectorToDirection(direction), step);
                if (nextSquare.hasPiece()) {
                    return nextSquare.currentPiece == piece;
                }
            }
        }
        return false;
    }

    bool sharesSameLineWith(Piece piece) {
        if (position.x == piece.position.x || position.y == piece.position.y) return true;
        var direction = piece.position - position;
        return BoardDirection.isDiagonal(direction);
    }

    bool getDirectionTo(Piece piece, out Vector2Int normalizedDirection) {
        if (sharesSameLineWith(piece)) {
            var direction = piece.position - position;
            if (BoardDirection.isDiagonal(direction)) {
                normalizedDirection = direction / Mathf.Abs(direction.x);
                return true;
            }
            var scalar = direction.x == 0 ? Mathf.Abs(direction.y) : Mathf.Abs(direction.x);
            normalizedDirection = direction / scalar;
            return true;
        }
        normalizedDirection = Vector2Int.zero;
        return false;
    }
    
    protected Vector2Int getRelativeDirection(PieceDirection relative) {
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

    PieceDirection vectorToDirection(Vector2Int vector) {
        PieceDirection direction;
        if (vector == BoardDirection.Up) {
            direction = PieceDirection.Forward;
        } else if (vector == BoardDirection.UpRight) {
            direction = PieceDirection.ForwardRight;
        } else if (vector == BoardDirection.Right) {
            direction = PieceDirection.Right;
        } else if (vector == BoardDirection.DownRight) {
            direction = PieceDirection.BackwardRight;
        } else if (vector == BoardDirection.Down) {
            direction = PieceDirection.Backward;
        } else if (vector == BoardDirection.DownLeft) {
            direction = PieceDirection.BackwardLeft;
        } else if (vector == BoardDirection.Left) {
            direction = PieceDirection.Left;
        } else if (vector == BoardDirection.UpLeft) {
            direction = PieceDirection.ForwardLeft;
        } else {
            throw new ArgumentException($"vector {vector} is invalid to cast to PieceDirection");
        }
        return isBottom ? direction : direction.getOpposite();
    }
    
    protected bool isSameSide(Piece other) => isWhite == other.isWhite;
    #endregion
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