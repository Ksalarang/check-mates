using System.Collections.Generic;
using GameScene.Models.BoardModel;

namespace GameScene.Models.Pieces {
public class Queen : Piece {
    public override List<Square> getAvailableSquares() {
        var list = new List<Square>();
        return list;
    }
}
}