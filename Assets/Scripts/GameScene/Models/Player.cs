using System.Collections.Generic;
using GameScene.Models.Pieces;

namespace GameScene.Models {
public class Player {
    public bool isWhite;
    public readonly List<Piece> pieces = new();

    public override string ToString() {
        var color = isWhite ? "w" : "b";
        return $"{color}_player";
    }
}
}