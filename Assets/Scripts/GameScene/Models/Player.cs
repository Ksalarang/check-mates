namespace GameScene.Models {
public class Player {
    public bool isWhite;

    public override string ToString() {
        var color = isWhite ? "w" : "b";
        return $"{color}_player";
    }
}
}