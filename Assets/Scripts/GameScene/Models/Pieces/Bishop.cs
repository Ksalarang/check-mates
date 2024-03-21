using System.Collections.Generic;
using GameScene.Models.BoardModel;

namespace GameScene.Models.Pieces {
public class Bishop : Piece {
    List<PieceDirection> directions;

    protected override void awake() {
        directions = PieceDirectionExt.getDiagonalDirections();
    }

    public override List<Square> getAvailableSquares() {
        var list = getAvailableSquaresInDirections(directions);
        checkAbsolutePin(list);
        return list;
    }
}
}