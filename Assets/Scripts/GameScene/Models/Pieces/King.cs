using System.Collections.Generic;
using GameScene.Models.BoardModel;

namespace GameScene.Models.Pieces {
public class King : Piece {
    List<PieceDirection> directions;

    protected override void awake() {
        directions = PieceDirectionExt.getAllDirections();
    }

    public override List<Square> getAvailableSquares() {
        var list = new List<Square>();
        foreach (var direction in directions) {
            var square = getSquareInDirection(direction);
            if (square is null) continue;
            
            if (!square.hasPiece()) {
                list.Add(square);
            } else if (!isSameSide(square.currentPiece) && square.currentPiece.type != PieceType.King) {
                list.Add(square);
            }
        }
        return list;
    }
}
}