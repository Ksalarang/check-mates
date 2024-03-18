using System.Collections.Generic;
using GameScene.Models.BoardModel;

namespace GameScene.Models.Pieces {
public class Rook : Piece {
    List<PieceDirection> directions;

    protected override void awake() {
        directions = PieceDirectionExt.getStraightDirections();
    }

    public override List<Square> getAvailableSquares() {
        return getAvailableSquaresInDirections(directions);
    }
}
}