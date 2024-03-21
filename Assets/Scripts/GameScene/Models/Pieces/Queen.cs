using System.Collections.Generic;
using GameScene.Models.BoardModel;

namespace GameScene.Models.Pieces {
public class Queen : Piece {
    List<PieceDirection> directions;

    protected override void awake() {
        directions = PieceDirectionExt.getAllDirections();
    }

    public override List<Square> getAvailableSquares() {
        var list = getAvailableSquaresInDirections(directions);
        checkAbsolutePin(list);
        return list;
    }
}
}