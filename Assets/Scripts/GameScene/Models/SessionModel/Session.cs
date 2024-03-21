using GameScene.Models.Pieces;

namespace GameScene.Models.SessionModel {
public interface Session {
    public Turn getLastTurn();

    public King getKing(bool white);
}
}