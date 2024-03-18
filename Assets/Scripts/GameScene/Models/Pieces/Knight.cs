using System.Collections.Generic;
using GameScene.Models.BoardModel;

namespace GameScene.Models.Pieces {
public class Knight : Piece {
    List<PieceDirection> directions;
    List<PieceDirection> LTurnDirections;

    protected override void awake() {
        directions = PieceDirectionExt.getStraightDirections();
        LTurnDirections = new List<PieceDirection> {
            PieceDirection.Left, PieceDirection.Right,
            PieceDirection.Forward, PieceDirection.Backward,
            PieceDirection.Right, PieceDirection.Left,
            PieceDirection.Backward, PieceDirection.Forward
        };
    }

    public override List<Square> getAvailableSquares() {
        var list = new List<Square>();

        var i = 0;
        foreach (var direction in directions) {
            var squarePosition = position + getRelativeDirection(direction) * 2;
            for (var j = 0; j < 2; j++) {
                var square = board.getSquare(squarePosition + getRelativeDirection(LTurnDirections[i++]));
                if (square is null) continue;
                if (!square.hasPiece()) {
                    list.Add(square);
                } else if (!isSameSide(square.currentPiece) && square.currentPiece.type != PieceType.King) {
                    list.Add(square);
                }
            }
        }
        
        return list;
    }
}
}