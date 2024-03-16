using UnityEngine;

namespace GameScene.Models.BoardModel {
public interface Board {
    public const int Size = 8;

    public Square getSquare(Vector2Int indices);

    public Square getSquare(int x, int y);

    public Square getSquareRelativeToOrigin(Vector2Int origin, Vector2Int direction, int steps);

    public static int getOppositeIndex(int index) => Size - 1 - index;
}
}