using System;

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
}
}