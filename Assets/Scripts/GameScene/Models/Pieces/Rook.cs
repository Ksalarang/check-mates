using System.Collections.Generic;
using GameScene.Models.BoardModel;

namespace GameScene.Models.Pieces {
public class Rook : Piece {
    List<PieceDirection> directions;

    protected override void awake() {
        directions = new List<PieceDirection> {
            PieceDirection.Forward, PieceDirection.Right, PieceDirection.Backward, PieceDirection.Left
        };
    }

    public override List<Square> getAvailableSquares() {
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
}
}