using System;
using System.Collections.Generic;

namespace GameScene.Models.Pieces {
public enum PieceDirection {
    Forward,
    ForwardRight,
    Right,
    BackwardRight,
    Backward,
    BackwardLeft,
    Left,
    ForwardLeft
}

public static class PieceDirectionExt {
    public static PieceDirection getOpposite(this PieceDirection direction) {
        return direction switch {
            PieceDirection.Forward => PieceDirection.Backward,
            PieceDirection.ForwardRight => PieceDirection.BackwardLeft,
            PieceDirection.Right => PieceDirection.Left,
            PieceDirection.BackwardRight => PieceDirection.ForwardLeft,
            PieceDirection.Backward => PieceDirection.Forward,
            PieceDirection.BackwardLeft => PieceDirection.ForwardRight,
            PieceDirection.Left => PieceDirection.Right,
            PieceDirection.ForwardLeft => PieceDirection.BackwardRight,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    public static List<PieceDirection> getStraightDirections() {
        return new List<PieceDirection> {
            PieceDirection.Forward, PieceDirection.Right, PieceDirection.Backward, PieceDirection.Left
        };
    }

    public static List<PieceDirection> getDiagonalDirections() {
        return new List<PieceDirection> {
            PieceDirection.ForwardRight, PieceDirection.BackwardRight,
            PieceDirection.BackwardLeft, PieceDirection.ForwardLeft
        };
    }

    public static List<PieceDirection> getAllDirections() {
        return new List<PieceDirection> {
            PieceDirection.Forward, PieceDirection.ForwardRight, PieceDirection.Right, PieceDirection.BackwardRight,
            PieceDirection.Backward, PieceDirection.BackwardLeft, PieceDirection.Left, PieceDirection.ForwardLeft
        };
    }
}
}